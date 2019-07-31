using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace SmartSql.DataConnector
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello SmartSql.DataConnector!");
            await new HostBuilder().Build().RunAsync();
        }
    }
}