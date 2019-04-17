using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Validators;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Test.Performance.Query
{
    public class QueryConfig : ManualConfig
    {
        public const int Iterations = 500;

        public QueryConfig()
        {
            Add(JitOptimizationsValidator.DontFailOnError);
            Add(ExecutionValidator.DontFailOnError);
            Add(ConsoleLogger.Default);

            Add(CsvExporter.Default);
            Add(MarkdownExporter.GitHub);
            Add(HtmlExporter.Default);

            Add(MemoryDiagnoser.Default);
            Add(TargetMethodColumn.Method);
            Add(StatisticColumn.Mean);

            Add(Job.ShortRun
                   .WithLaunchCount(1)
                   .WithWarmupCount(2)
                   .WithUnrollFactor(Iterations)
                   .WithIterationCount(1)
            );
        }
    }
}
