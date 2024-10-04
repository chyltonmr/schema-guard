using Avro;
using Avro.Generic;
using Avro.IO;
using Avro.Specific;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Newtonsoft.Json;


public class SchemaValidator
{

    public bool ValidarEstruturaJsonUsandoClasse(string json, Type tipoEsperado)
    {
        try
        {
            // Deserializar o JSON em um JObject para navegação dinâmica
            JObject jsonObj = JObject.Parse(json);

            // Chamar a função de validação recursiva para verificar todos os campos da classe tipada
            return ValidarCamposRecursivamente(jsonObj, tipoEsperado, jsonObj);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro de serialização ou estrutura inválida: {ex.Message}");
            return false;
        }
    }

    private bool ValidarCamposRecursivamente(JToken jsonToken, Type tipoEsperado, JToken tokenRaiz)
    {
        foreach (PropertyInfo propriedade in tipoEsperado.GetProperties())
        {
            string nomeCampo = propriedade.Name;
            Type tipoCampo = propriedade.PropertyType;

            // Buscar o token diretamente da propriedade no token atual
            JToken token = jsonToken[nomeCampo];

            // Se o token for nulo, verificar se é uma lista ou objeto e montar o caminho completo
            if (token == null)
            {
                token = tokenRaiz.SelectToken(nomeCampo, errorWhenNoMatch: false);
                if (token == null)
                {
                    Console.WriteLine($"Campo '{nomeCampo}' não encontrado.");
                    return false;
                }
            }

            // Verificar se é uma string, tipo primitivo ou valor
            if (tipoCampo == typeof(string) || tipoCampo.IsPrimitive || tipoCampo.IsValueType)
            {
                // Salvar o nome da propriedade e seu valor
                string nomePropriedade = nomeCampo;
                string valorPropriedade = token.ToString();

                // Exibir o nome e o valor da propriedade
                Console.WriteLine($"Propriedade '{nomePropriedade}' é do tipo {tipoCampo.Name} com valor: {valorPropriedade}");

                // Você pode usar essas variáveis conforme necessário.
                continue; // Continuar para a próxima propriedade
            }

            // Se for uma propriedade complexa (objeto ou lista), validar recursivamente
            if (tipoCampo.IsGenericType)
            {
                bool isEnumerable = typeof(IEnumerable<>).IsAssignableFrom(tipoCampo.GetGenericTypeDefinition());
                bool isList = tipoCampo.GetGenericTypeDefinition() == typeof(List<>);

                if (isEnumerable || isList)
                {
                    Console.WriteLine($"O campo '{nomeCampo}' é uma coleção (IEnumerable ou List).");

                    // Processar os itens da coleção
                    foreach (var item in token.Children())
                    {
                        var tipoItem = tipoCampo.GetGenericArguments()[0];
                        if (!ValidarCamposRecursivamente(item, tipoItem, tokenRaiz))
                        {
                            return false;
                        }
                    }
                    continue;
                }
            }
            else if (tipoCampo.IsClass && tipoCampo != typeof(string))
            {
                // Verificar se é um objeto
                Console.WriteLine($"O campo '{nomeCampo}' é um objeto.");

                // Processar recursivamente os campos do objeto
                if (!ValidarCamposRecursivamente(token, tipoCampo, tokenRaiz))
                {
                    return false;
                }
            }
            else
            {
                // Verificar se o tipo corresponde ao esperado para tipos simples
                if (token.Type == JTokenType.Null || !token.ToObject(tipoCampo).GetType().IsAssignableFrom(tipoCampo))
                {
                    Console.WriteLine($"Campo '{nomeCampo}' tem tipo inválido. Esperado: {tipoCampo}, Encontrado: {token.Type}");
                    return false;
                }
            }
        }

        // Se todas as validações forem bem-sucedidas
        return true;
    }
}

public class SchemaField
{
    public string Name { get; set; }
}
