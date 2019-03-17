using BenchmarkDotNet.Running;
using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using SmartSql.Test.Performance.Query;

namespace SmartSql.Test.Performance
{
    class Program
    {
        static void Main(string[] args)
        {
            //new SmartSqlTest()
            //    .Setup();
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }
}
