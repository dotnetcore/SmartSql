using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;

namespace SmartSql.PerformanceTests
{
    [MinColumn, MaxColumn, RankColumn]
    [MemoryDiagnoser]
    [MarkdownExporter, AsciiDocExporter, HtmlExporter, CsvExporter, RPlotExporter]
    public class CreateInstance_Test
    {
        [GlobalSetup]
        public void Setup()
        {
        }
        [Benchmark]
        public void New()
        {
            var entity = new T_Entity();
        }
        [Benchmark]
        public void New_Activator()
        {
            var entity = Activator.CreateInstance(typeof(T_Entity));
        }
    }
}
