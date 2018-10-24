using BenchmarkDotNet.Running;

namespace SmartSql.PerformanceTests
{
    class Program
    {

        static void Main(string[] args)
        {
            new BenchmarkSwitcher(typeof(BenchmarkBase).Assembly).Run(args, new Config());
        }
    }

}
