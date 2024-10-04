
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using schema_guard;
using System;
using System.Threading.Tasks;

namespace SeuProjeto
{
    public class MeuServico
    {

        private readonly AppSettingsSchemaGuard _appsetings;
        private readonly IServiceProvider _serviceProvider;
        private readonly SchemaValidator _schemaValidator;

        public MeuServico(IOptionsSnapshot<AppSettingsSchemaGuard> optionsSnapshot, SchemaValidator schemaValidator)
        {
            this._appsetings = optionsSnapshot.Value;
            this._schemaValidator = schemaValidator;
        }



        public async Task<string> Executar()
        {
            string jsonRecebido = @"
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

            // Validar o JSON com base na classe Root
            bool resultadoValidacao = _schemaValidator.ValidarEstruturaJsonUsandoClasse(jsonRecebido, typeof(Root));

            if (resultadoValidacao)
            {
                Console.WriteLine("JSON válido.");
            }
            else
            {
                Console.WriteLine("JSON inválido.");
            }

            return "";
        }
    }
}