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
            // Criação do Host
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // Configurar as opções
                    services.Configure<AppSettings>(context.Configuration.GetSection("AppSettings"));
                    // Registrar a classe que usará IOptionsSnapshot
                    services.AddTransient<MeuServico>();
                })
                .Build();

            // Resolve e executa o serviço
            var meuServico = host.Services.GetRequiredService<MeuServico>();
           var result = meuServico.Executar();

            // Aguarde uma tecla para fechar
            Console.ReadKey();
        }
    }
}
