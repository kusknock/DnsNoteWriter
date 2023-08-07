using DnsNoteWriter.Services;
using DnsNoteWriter.Services.Interfaces;
using DnsNoteWriter.Transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace DnsNoteWriter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();
            using var scope = host.Services.CreateScope();

            var services = scope.ServiceProvider;

            try
            {
                services.GetRequiredService<App>().Run(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            IHostBuilder CreateHostBuilder(string[] strings)
            {
                return Host.CreateDefaultBuilder()
                    .ConfigureServices((_, services) =>
                    {
                        services.AddSingleton<App>();
                        services.AddSingleton<GatewayClient>();
                        services.AddScoped<INoteWriter, NoteWriter>();
                        services.AddScoped<NoteWriterService>();
                    })
                    .ConfigureAppConfiguration(app =>
                    {
                        //app.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
                        app.AddJsonFile("appsettings.json");
                    });
            }
        }
    }
}