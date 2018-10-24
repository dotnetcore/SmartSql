using BenchmarkDotNet.Attributes;
using Chloe.SqlServer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SmartSql.PerformanceTests
{
    [Description("Chloe")]
    public class ChloeBenchmarks : BenchmarkBase
    {
        MsSqlContext _chloeContext;
        [GlobalSetup]
        public void Setup()
        {
            _chloeContext = new MsSqlContext(ConnectionString);
        }
        [GlobalCleanup]
        public void Cleanup()
        {
            _chloeContext.Dispose();
        }
        //[Benchmark]
        public override long Insert()
        {
            var entity = GetInsertEntity();
            var newEntity = _chloeContext.Insert<T_Entity>(entity);

            return newEntity.FLong;
        }
        [Benchmark]
        public override IEnumerable<T_Entity> Query()
        {
            var list = _chloeContext.Query<T_Entity>().Take(QUERY_TAKEN).ToList();
            return list;
        }

        public override int Update()
        {
            throw new NotImplementedException();
        }

    }
}
