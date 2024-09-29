
using Microsoft.Extensions.Options;

namespace SeuProjeto
{
    public class MeuServico
    {

        private readonly AppSettings _appsetings;

        public MeuServico(IOptionsSnapshot<AppSettings> appSettings)
        {

            _appsetings = appSettings.Value; // Obtém a instância atual das opções
        }



        public string Executar()
        {
            return _appsetings.Title;
        }
    }
}