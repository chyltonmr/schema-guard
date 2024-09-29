
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace SeuProjeto
{
    public class MeuServico
    {

        private readonly AppSettings _appsetings;
        private readonly IServiceProvider _serviceProvider;

        public MeuServico(IServiceProvider serviceProvider)
        {

            _serviceProvider = serviceProvider;
            //_appsetings = appSettings.Value; // Obtém a instância atual das opções
        }



        public string Executar()
        {
            // 3. Resolver a instância
            var myService = _serviceProvider.GetService<IOptionsSnapshot<AppSettings>>();

            return myService.Value.Title;
        }
    }
}