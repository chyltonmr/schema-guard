using Avro;
using Avro.Generic;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

public class SchemaValidator
{
    private readonly string _expectedSchemaJson = @"{
        ""type"": ""record"",
        ""name"": ""Customer"",
        ""namespace"": ""com.example"",
        ""fields"": [
            {""name"": ""id"", ""type"": ""string""},
            {""name"": ""name"", ""type"": ""string""},
            {""name"": ""email"", ""type"": ""string""},
            {""name"": ""age"", ""type"": [""null"", ""int""], ""default"": null},
            {
                ""name"": ""conta_corrente"",
                ""type"": {
                    ""type"": ""record"",
                    ""name"": ""ContaCorrente"",
                    ""fields"": [
                        {""name"": ""banco"", ""type"": ""string""},
                        {""name"": ""agencia"", ""type"": ""string""},
                        {""name"": ""numero_conta"", ""type"": ""string""}
                    ]
                }
            }
        ]
    }";

    public void ValidateSchema()
    {
        // Parsear o schema Avro
        var schema = (RecordSchema)Schema.Parse(_expectedSchemaJson);
        var esperado = new GenericRecord(schema);


        // Gerar dados fake para o GenericRecord
        var fakeData = new AvroFakeDataGenerator();
        var rrg4 = fakeData.GenerateFakeGenericRecord(fakeData.schemaJson);

        foreach (var field in esperado.Schema.Fields)
        {



            if (!rrg4.Schema.Fields.Contains(field))
            {
                throw new InvalidOperationException($"Campo {field.Name} não existe no schema atual!");
            }else
            {
                var s = field.Schema;

                if (s is PrimitiveSchema)
                {
                    Console.WriteLine($"Tipo: Primitivo ({s.Fullname})");
                }
                else if (s is RecordSchema)
                {
                    Console.WriteLine("Tipo: Objeto (Record)");
                }
                else if (s is ArraySchema)
                {
                    Console.WriteLine("Tipo: Lista (Array)");
                }
                else if (s is UnionSchema)
                {
                    Console.WriteLine("Tipo: Union (Possui múltiplos tipos)");
                    // Verificar tipos dentro da união
                    foreach (var subType in ((UnionSchema)s).Schemas)
                    {
                        Console.WriteLine($" - Subtipo: {subType.Fullname}");
                    }
                }
                else
                {
                    Console.WriteLine($"Tipo desconhecido: {s.GetType().Name}");
                }
            }

            //// Validação de objeto embutido (conta_corrente)
            //if (field.Name == "conta_corrente")
            //{
            //    var contaCorrenteField = record["conta_corrente"] as GenericRecord;
            //    if (contaCorrenteField != null)
            //    {
            //        ValidateContaCorrente(contaCorrenteField);
            //    }
            //    else
            //    {
            //        throw new InvalidOperationException("Campo conta_corrente está ausente ou malformado no schema atual.");
            //    }
            //}
        }
    }

    private void ValidateContaCorrente(GenericRecord contaCorrente)
    {
        var expectedContaCorrenteFields = new List<string> { "banco", "agencia", "numero_conta" };

        //foreach (var field in expectedContaCorrenteFields)
        //{
        //    if (!contaCorrente.Schema.Fields.Contains(field))
        //    {
        //        throw new InvalidOperationException($"Campo {field} não existe na estrutura conta_corrente!");
        //    }
        //}
    }
}

public class SchemaField
{
    public string Name { get; set; }
}
