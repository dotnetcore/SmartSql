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
        static void Main(string[] args)
        {
            Console.WriteLine("------******  Hello SmartSql.DataConnector!  ******------");
            new HostBuilder()
                .ConfigureServices((hostingContext, services) =>
                {
                    services.AddOptions();
                    services.Configure<AppOptions>(hostingContext.Configuration.GetSection("App"));
                    services.AddHostedService<AppService>();
                }).ConfigureAppConfiguration(builder =>
                {
                    builder.AddJsonFile("appsettings.json").AddEnvironmentVariables();
                    ;
                })
                .UseSerilog((hostingContext, loggerConfiguration) =>
                {
                    loggerConfiguration
                        .ReadFrom.Configuration(hostingContext.Configuration)
                        .Enrich.FromLogContext();
                })
                .Build().Run();
        }
    }
}