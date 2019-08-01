using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SmartSql.DataConnector
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello SmartSql.DataConnector!");
            await new HostBuilder()
                .ConfigureServices((builder,services) =>
                {
                    services.AddOptions();
                    services.Configure<AppOptions>(builder.Configuration.GetSection("App"));
                    services.AddHostedService<AppService>(); 
                    
                }).ConfigureAppConfiguration(builder =>
                {
                    builder.AddJsonFile("appsettings.json");
                })
                .Build().RunAsync();
        }
    }
}