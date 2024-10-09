﻿using Avro;
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
            TraverseSchema(esperado.Schema, dadosFakes);
        }
    }

    // Função recursiva para percorrer o schema
    private static void TraverseSchema(Schema schemaEsperado, GenericRecord dadosFakes, int indentLevel = 0)
    {
        string indent = new string(' ', indentLevel * 2);  // Controle da indentação para melhor visualização


        if (schemaEsperado is RecordSchema recordSchemaEsperado)
        {
            Console.WriteLine($"{indent}Objeto (Record): {recordSchemaEsperado.Fullname}");
            foreach (var fieldEsperados in recordSchemaEsperado.Fields)
            {
                Console.WriteLine($"{indent}  Campo: {fieldEsperados.Name}");

                // Verificar se o campo existe no GenericRecord 
                if (dadosFakes.TryGetValue(fieldEsperados.Name, out var fieldValue))
                {
                    Console.WriteLine($"{indent}    Valor encontrado: {fieldValue}");

                    // Verificar se o campo é um objeto e percorrer recursivamente
                    if (fieldEsperados.Schema is RecordSchema)
                    {
                        TraverseSchema(fieldEsperados.Schema, (GenericRecord)fieldValue, indentLevel + 2);
                    }
                    // Verificar se o campo é uma lista e validar seus itens
                    else if (fieldEsperados.Schema is ArraySchema arraySchema)
                    {
                        var items = fieldValue as IEnumerable<object>;
                        Console.WriteLine($"{indent}    Lista encontrada com {items?.ToString() ?? "0"} itens");
                        foreach (var item in items)
                        {
                            // Validar o tipo dos itens da lista
                            Console.WriteLine($"{indent}    Item: {item}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"{indent}    Campo não encontrado no GenericRecord");
                }
            }
        }
        else if (schemaEsperado is PrimitiveSchema primitiveSchema)
        {
            Console.WriteLine($"{indent}Tipo Primitivo: {primitiveSchema.Fullname}");
        }
        else if (schemaEsperado is ArraySchema arraySchema)
        {
            Console.WriteLine($"{indent}Lista (Array): Tipo dos itens: {arraySchema.ItemSchema.Fullname}");
        }
        else if (schemaEsperado is UnionSchema unionSchema)
        {
            Console.WriteLine($"{indent}Union (UnionSchema):");
            foreach (var subSchema in unionSchema.Schemas)
            {
                TraverseSchema(subSchema, dadosFakes, indentLevel + 2);
            }
        }
    }
}


    

