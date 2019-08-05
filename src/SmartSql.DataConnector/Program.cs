using System;
using System.Reflection;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace SmartSql.DataConnector
{
    [VersionOptionFromMember("-v|--version", MemberName = nameof(GetVersion))]
    public class Program
    {
        public const string DEFAULT_CONFIG_PATH = "appsettings.json";

        [Argument(0, Description = "Config Path")]
        [FileExists]
        public String ConfigPath { get; set; }

        static void Main(string[] args)
        {
            CommandLineApplication.Execute<Program>(args);
        }

        private async Task OnExecute()
        {
            Console.WriteLine("------******  Hello SmartSql.DataConnector!  ******------");
            if (String.IsNullOrEmpty(ConfigPath))
            {
                ConfigPath = DEFAULT_CONFIG_PATH;
                Console.WriteLine($"{nameof(ConfigPath)} is empty,Use the default configuration:[{DEFAULT_CONFIG_PATH}].");
            }

            await new HostBuilder()
                .ConfigureServices((hostingContext, services) =>
                {
                    services.AddOptions();
                    services.Configure<AppOptions>(hostingContext.Configuration.GetSection("App"));
                    services.AddHostedService<AppService>();
                }).ConfigureAppConfiguration(builder =>
                {
                    builder.AddJsonFile(ConfigPath).AddEnvironmentVariables();
                })
                .UseSerilog((hostingContext, loggerConfiguration) =>
                {
                    loggerConfiguration
                        .ReadFrom.Configuration(hostingContext.Configuration)
                        .Enrich.FromLogContext();
                })
                .Build().RunAsync();
        }

        private static string GetVersion()
            => typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion;
    }
}