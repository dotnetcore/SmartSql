using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using SmartSql.Bulk;
using SmartSql.Test.Entities;
using Xunit;
using Xunit.Abstractions;

namespace SmartSql.Test.Unit.Bulk
{
    public class BulkTest
    {
        private readonly ITestOutputHelper _output;

        public BulkTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ToDataTable()
        {
            var list = new List<AllPrimitive>();
            for (int i = 0; i < 100000; i++)
            {
                list.Add(new AllPrimitive
                {
                    Id = i
                });
            }

            var watch = Stopwatch.StartNew();
            var dataTableE1 = list.ToDataTable();
            _output.WriteLine($"ToDataTable taken :{watch.ElapsedMilliseconds}");
            watch.Stop();
        }
    }
}