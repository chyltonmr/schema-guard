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
using System.Net.NetworkInformation;
using schema_guard;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using schema_guard.TipoSchemas;
using schema_guard.Config;


public class ConectorLambda : IConectorLambda
{

    private readonly GuardSettings _appSettings;
    private readonly string _jsonRecebido;

    public ConectorLambda(IOptionsSnapshot<GuardSettings> optionsSnapshot)
    {
        _appSettings = optionsSnapshot.Value;

        #region Mock _appSettings

        _jsonRecebido = @"
{
  'date': '2024-01-01',
  'guid': '35ggrggg444',
  'level': 'Verbose',
  'message': [
    {
      'payload': {
        'key': 'd444ggg5g5',
        'value': {
          'data': {
            'texto_status_execucao': 'VALIDADA',
            'data_hora_evento': '2024-01-01 14:11 15:11',
            'descricao_status_operacao': 'VALIDADA',
            'descricao_contrato_operacao': 'ff4444-4f4444-45gfgfg4-333',
            'codigo_identificador_boleto': '3444444f4-rgrgrgr-44g444'
          }
        },
        'topic': 'tesouraria-coreografada-credito',
        'partition': 2,
        'offset': 33333,
        'timestamp': 344444,
        'headers': [
          {
            'key': 'type',
            'value': ''
          },
          {
            'key': 'source',
            'value': 'IC8'
          },
          {
            'key': 'id',
            'value': 'IC8-4566grf4g5g5g5g-gg55555'
          }
        ]
      }
    }
  ]
}";

        #endregion
    }

    public async Task<bool> Validar(string json, Type tipoEsperado)
    {
        //TODO: RETIRAR MOCK POSTERIORMENTE
        json = _jsonRecebido;
        //tipoEsperado = tipoEsperado;

        try
        {
            int totalPropriedadesRoot = tipoEsperado.GetProperties().Count();

            // Deserializar o JSON em um JObject para navegação dinâmica
            JObject jsonObj = JObject.Parse(json);

            // Chamar a função de validação recursiva para verificar todos os campos da classe tipada
            var resp = ValidarCamposRecursivamente(jsonObj, tipoEsperado, jsonObj, ref totalPropriedadesRoot);

            if (totalPropriedadesRoot == default(int) && resp.Any())
            {

                Console.WriteLine("JSON válido.");
                return true;
            }
            else
            {
                Console.WriteLine("JSON inválido.");
                return false;
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro de serialização ou estrutura inválida: {ex.Message}");
            return false;
        }
    }

    private Dictionary<string, string> ValidarCamposRecursivamente(JToken jsonToken, Type tipoEsperado, JToken tokenRaiz, ref int totalPropriedadesRoot)
    {
        var camposNaoExistem = new Dictionary<string, string>();

        foreach (PropertyInfo propriedade in tipoEsperado.GetProperties())
        {
            totalPropriedadesRoot = totalPropriedadesRoot - 1;

            string nomeCampo = propriedade.Name;
            Type tipoCampo = propriedade.PropertyType;

            JToken? token = BuscaToken(jsonToken, tokenRaiz, nomeCampo, ref camposNaoExistem);
            if (camposNaoExistem.Any() && camposNaoExistem[nomeCampo].Any())
                continue;


            if (TipoString(tipoCampo, nomeCampo, token, ref camposNaoExistem))
                continue;

            if (TipoClass(tipoCampo, nomeCampo, token, tokenRaiz, ref totalPropriedadesRoot))
                continue;

            if (TipoLista(tipoCampo, nomeCampo, token, tokenRaiz, ref totalPropriedadesRoot))
                continue;

            if (CampoInvalido(tipoCampo, nomeCampo, token, ref camposNaoExistem))
                continue;
        }

        return camposNaoExistem;
    }

    /// <summary>
    /// Buscar o token diretamente da propriedade no token atual
    /// </summary>
    /// <param name="jsonToken"></param>
    /// <param name="nomeCampo"></param>
    /// <param name="camposNaoExistentes"></param>
    private JToken? BuscaToken(JToken jsonToken, JToken tokenRaiz, string nomeCampo, ref Dictionary<string, string> camposNaoExistentes)
    {
        JToken? token = jsonToken[nomeCampo];

        if (token == null)
            camposNaoExistentes.Add(nomeCampo, nomeCampo);

        return token;
    }


    /// <summary>
    /// Verifica se um campo e do tipo string.
    /// </summary>
    /// <param name="tipoCampo"></param>
    /// <param name="nomeCampo"></param>
    /// <param name="token"></param>
    /// <param name="camposNaoExistentes"></param>
    /// <returns></returns>
    private bool TipoString(Type tipoCampo, string nomeCampo, JToken token, ref Dictionary<string, string> camposNaoExistentes)
    {
        // Verificar se é uma string, tipo primitivo ou tipo por valor
        if (tipoCampo == typeof(string) || tipoCampo.IsPrimitive || tipoCampo.IsValueType)
        {
            // Salvar o nome da propriedade e seu valor
            string nomePropriedade = nomeCampo;
            string valorPropriedade = token.ToString();

            // Exibir o nome e o valor da propriedade
            Console.WriteLine($"Propriedade '{nomePropriedade}' é do tipo {tipoCampo.Name} com valor: {valorPropriedade}");

            return true;
        }

        return false;
    }


    /// <summary>
    /// Se for uma propriedade complexa (objeto ou lista), validar recursivamente
    /// </summary>
    /// <param name="tipoCampo"></param>
    /// <param name="nomeCampo"></param>
    /// <param name="token"></param>
    /// <param name="tokenRaiz"></param>
    /// <returns></returns>
    private bool TipoLista(Type tipoCampo, string nomeCampo, JToken token, JToken tokenRaiz, ref int totalPropriedadesRoot)
    {

        bool isEnumerable = typeof(IEnumerable<>).IsAssignableFrom(tipoCampo?.GetGenericTypeDefinition());
        bool isList = tipoCampo?.GetGenericTypeDefinition() == typeof(List<>);

        //if (tipoCampo.IsClass && tipoCampo != typeof(string) && tipoCampo != typeof(IEnumerable<>) && tipoCampo != typeof(List<>))

        if (isEnumerable || isList)
        {
            Console.WriteLine($"O campo '{nomeCampo}' é uma coleção (IEnumerable ou List).");

            // Processar os itens da coleção
            foreach (var item in token.Children())
            {
                var tipoItem = tipoCampo.GetGenericArguments()[0];
                ValidarCamposRecursivamente(item, tipoItem, tokenRaiz, ref totalPropriedadesRoot);
            }

            return true;
        }

        return false;
    }

    private bool TipoClass(Type tipoCampo, string nomeCampo, JToken token, JToken tokenRaiz, ref int totalPropriedadesRoot)
    {

        if (tipoCampo.IsClass && tipoCampo?.IsGenericType != null && tipoCampo?.IsGenericType != true)
        {
            // Verificar se é um objeto
            Console.WriteLine($"O campo '{nomeCampo}' é um objeto.");

            // Processar recursivamente os campos do objeto
            ValidarCamposRecursivamente(token, tipoCampo, tokenRaiz, ref totalPropriedadesRoot);

            return true;
        }

        return false;
    }

    private bool CampoInvalido(Type tipoCampo, string nomeCampo, JToken token, ref Dictionary<string, string> camposNaoExistentes)
    {
        // Verificar se o tipo corresponde ao esperado para tipos simples
        if (token.Type == JTokenType.Null || !token.ToObject(tipoCampo).GetType().IsAssignableFrom(tipoCampo))
        {
            Console.WriteLine($"Campo '{nomeCampo}' tem tipo inválido. Esperado: {tipoCampo}, Encontrado: {token.Type}");

            camposNaoExistentes.Add(nomeCampo, $"Campo '{nomeCampo}' tem tipo inválido. Esperado: {tipoCampo}, Encontrado: {token.Type}");


            return true;
        }

        return false;
    }
}


