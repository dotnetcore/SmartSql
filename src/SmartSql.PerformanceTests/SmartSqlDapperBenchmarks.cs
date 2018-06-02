using BenchmarkDotNet.Attributes;
using SmartSql.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartSql.PerformanceTests
{
    public class SmartSqlDapperBenchmarks : BenchmarkBase
    {
        ISmartSqlMapper _sqlMapper;
        [GlobalSetup]
        public void Setup()
        {
            _sqlMapper = new SmartSqlMapper(new SmartSqlOptions
            {
                DataReaderDeserializerFactory = new DapperDeserializer.DapperDataReaderDeserializerFactory()
            });
        }
        [GlobalCleanup]
        public void Cleanup()
        {
            _sqlMapper.Dispose();
        }

        public override long Insert()
        {
            throw new NotImplementedException();
        }
        [Benchmark]
        public override IEnumerable<T_Entity> Query()
        {
            var list = _sqlMapper.Query<T_Entity>(new RequestContext
            {
                Scope = "Entity",
                SqlId = "Query",
                Request = new { Taken = QUERY_TAKEN }
            });
            return list;
        }

        public override int Update()
        {
            throw new NotImplementedException();
        }


    }
}
