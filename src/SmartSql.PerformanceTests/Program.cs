using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;

namespace SmartSql.PerformanceTests
{
    class Program
    {

        static void Main(string[] args)
        {
            var benchmarks = new List<Benchmark>();
            var benchTypes = Assembly.GetEntryAssembly().DefinedTypes.Where(t => t.IsSubclassOf(typeof(BenchmarkBase)));
            foreach (var b in benchTypes)
            {
                benchmarks.AddRange(BenchmarkConverter.TypeToBenchmarks(b).Benchmarks);
            }
            BenchmarkRunner.Run(benchmarks.ToArray(), null);
        }
    }

}
