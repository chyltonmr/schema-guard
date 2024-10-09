using Avro;
using Avro.Generic;
using System;
using System.Collections.Generic;
using System.Linq;

public class AvroFakeDataGenerator
{

    public string schemaJson;
    public AvroFakeDataGenerator()
    {
        // Definir o schema Avro (JSON Schema)
        schemaJson = @"
        {
            ""type"": ""record"",
            ""name"": ""Customer"",
            ""namespace"": ""com.example"",
            ""fields"": [
                { ""name"": ""id"", ""type"": ""string"" },
                { ""name"": ""name"", ""type"": ""string"" },
                { ""name"": ""email"", ""type"": ""string"" },
                { ""name"": ""age"", ""type"": [""null"", ""int""], ""default"": null },
                { 
                    ""name"": ""conta_corrente"",
                    ""type"": {
                        ""type"": ""record"",
                        ""name"": ""ContaCorrente"",
                        ""fields"": [
                            { ""name"": ""banco"", ""type"": ""string"" },
                            { ""name"": ""agencia"", ""type"": ""string"" },
                            { ""name"": ""numero_conta"", ""type"": ""string"" }
                        ]
                    }
                }
            ]
        }";

       
      

    }

    public GenericRecord GenerateFakeGenericRecord(string jsonSchema)
    {

        // Parsear o schema Avro
        var schema = (RecordSchema)Schema.Parse(jsonSchema);


        // Criar o GenericRecord com base no schema
        var record = new GenericRecord(schema);

        // Atribuir valores fake para os campos
        record.Add("id", "sVgr*1403");
        record.Add("name", "John Doe");
        record.Add("email", "johndoe@example.com");
        record.Add("age", 30);

        // Acessar o campo "conta_corrente" usando o método correto para acessar os fields
        Field contaCorrenteField = schema.Fields.Find(f => f.Name == "conta_corrente");
        var contaCorrenteSchema = (RecordSchema)contaCorrenteField.Schema;

        // Criar dados fake para o campo "conta_corrente"
        var contaCorrente = new GenericRecord(contaCorrenteSchema);
        contaCorrente.Add("banco", "123");
        contaCorrente.Add("agencia", "4567");
        contaCorrente.Add("numero_conta", "890123");

        // Adicionar "conta_corrente" no registro principal
        record.Add("conta_corrente", contaCorrente);


        ///


          // Exibir os dados fake
        Console.WriteLine($"ID: {record["id"]}");
        Console.WriteLine($"Nome: {record["name"]}");
        Console.WriteLine($"Email: {record["email"]}");
        Console.WriteLine($"Idade: {record["age"]}");

        var respContaCorrente = record["conta_corrente"] as GenericRecord;
        if (respContaCorrente != null)
        {
            Console.WriteLine($"Banco: {respContaCorrente["banco"]}");
            Console.WriteLine($"Agência: {respContaCorrente["agencia"]}");
            Console.WriteLine($"Número da Conta: {respContaCorrente["numero_conta"]}");
        }

        return record;
    }
}


