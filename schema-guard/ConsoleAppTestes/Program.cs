using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using schema_guard;

namespace SeuProjeto
{
    class Program
    {
        static void Main(string[] args)
        {
            // Configurar o builder para carregar appsettings.json
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = configurationBuilder.Build();

            // Criar o ServiceCollection e adicionar as configurações
            var services = new ServiceCollection();

            //Criar o mapeamento da secao "AppSettings" do appsettins para class da nossa lib "AppSettingsSchemaGuard". 
            //Dessa forma, posteriormente internamente em nossa class "MeuServico" da lib, sera possível regatar mapemaento
            //atraves de 'IOptionsSnapshot'
            services.Configure<AppSettingsSchemaGuard>(configuration.GetSection("AppSettings"));

            services.AddTransient<MeuServico>();

            // Construir o ServiceProvider
            var serviceProvider = services.BuildServiceProvider();

            // Resolve e executa o serviço
            var meuServico = serviceProvider.GetService<MeuServico>();
            var result = meuServico.Executar();

            // Aguarde uma tecla para fechar
            Console.ReadKey();
        }
    }
}
