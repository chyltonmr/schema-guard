using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
            services.AddTransient<MeuServico>(provider => new MeuServico(services.BuildServiceProvider()));

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
