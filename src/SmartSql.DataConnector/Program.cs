using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace SmartSql.DataConnector
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello SmartSql.DataConnector!");
            await new HostBuilder()
                .ConfigureServices((hostingContext, services) =>
                {
                    services.AddOptions();
                    services.Configure<AppOptions>(hostingContext.Configuration.GetSection("App"));
                    services.AddHostedService<AppService>();
                }).ConfigureAppConfiguration(builder => { builder.AddJsonFile("appsettings.json"); })
                .UseSerilog((hostingContext, loggerConfiguration) =>
                {
                   var filePath= hostingContext.Configuration.GetValue<string>("Serilog:File:Path");
                    loggerConfiguration
                        .ReadFrom.Configuration(hostingContext.Configuration)
                        .Enrich.FromLogContext()
                        .WriteTo.Console()
                        .WriteTo.File(filePath,
                            fileSizeLimitBytes: 1_000_000,
                            rollOnFileSizeLimit: true,
                            shared: true,
                            flushToDiskInterval: TimeSpan.FromSeconds(1));
                })
                .Build().RunAsync();
        }
    }
}