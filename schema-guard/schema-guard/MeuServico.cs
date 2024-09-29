
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

        //public MeuServico(IServiceProvider serviceProvider)
        //{

        //    _serviceProvider = serviceProvider;
        //    //_appsetings = appSettings.Value; // Obtém a instância atual das opções
        //}

        public MeuServico(IOptionsSnapshot<AppSettingsSchemaGuard> optionsSnapshot)
        {

            try
            {
                _appsetings = optionsSnapshot.Value;
                //_appsetings = appSettings.Value; // Obtém a instância atual das opções

            }
            catch (Exception ex)
            {

                throw;
            }
        }



        public async Task<string> Executar()
        {
            // 3. Resolver a instância
            //var myService = _serviceProvider.GetService<IOptionsSnapshot<AppSettingsSchemaGuard>>();

            var git = new GitHubAvroDownloader();
            await git.GetAvroFileFromGitHub();

            return _appsetings.Title;
        }
    }
}