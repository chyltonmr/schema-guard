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
        var dadosFakes = fakeData.GenerateFakeGenericRecord(fakeData.schemaJson);

        foreach (var field in esperado.Schema.Fields)
        {



            if (!dadosFakes.Schema.Fields.Contains(field))
            {
                throw new InvalidOperationException($"Campo {field.Name} não existe no schema atual!");
            }

            // Percorrer os campos do schema
            TraverseSchema(esperado.Schema);
        }
    }

    // Função recursiva para percorrer o schema
    private static void TraverseSchema(Schema schema, int indentLevel = 0)
    {
        string indent = new string(' ', indentLevel * 2);  // Controle da indentação para melhor visualização

        // Verificar o tipo do schema
        if (schema is RecordSchema recordSchema)
        {
            Console.WriteLine($"{indent}Objeto (Record): {recordSchema.Fullname}");
            foreach (var field in recordSchema.Fields)
            {
                Console.WriteLine($"{indent}  Campo: {field.Name}");
                TraverseSchema(field.Schema, indentLevel + 2);  // Percorre os campos do objeto (recursivo)
            }
        }
        else if (schema is ArraySchema arraySchema)
        {
            Console.WriteLine($"{indent}Lista (Array):");
            Console.WriteLine($"{indent}  Tipo dos itens:");
            TraverseSchema(arraySchema.ItemSchema, indentLevel + 2);  // Percorre os itens do array
        }
        else if (schema is UnionSchema unionSchema)
        {
            Console.WriteLine($"{indent}Union (UnionSchema):");
            foreach (var subSchema in unionSchema.Schemas)
            {
                Console.WriteLine($"{indent}  Subtipo:");
                TraverseSchema(subSchema, indentLevel + 2);  // Percorre cada subtipo da união
            }
        }
        else if (schema is PrimitiveSchema primitiveSchema)
        {
            Console.WriteLine($"{indent}Tipo Primitivo: {primitiveSchema.Fullname}");
        }
        else
        {
            Console.WriteLine($"{indent}Tipo desconhecido: {schema.GetType().Name}");
        }
    }
}

    

