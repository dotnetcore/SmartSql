using BenchmarkDotNet.Attributes;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SmartSql.PerformanceTests
{
    [Description("SqlSugar")]
    public class SqlSugarBenchmarks : BenchmarkBase
    {
        SqlSugarClient _sqlSugarClient;
        [GlobalSetup]
        public void Setup()
        {
            _sqlSugarClient = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = ConnectionString,
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true
            });
        }
        [GlobalCleanup]
        public void Cleanup()
        {
            _sqlSugarClient.Dispose();
        }
        public override long Insert()
        {
            throw new NotImplementedException();
        }

        [Benchmark]
        public override IEnumerable<T_Entity> Query()
        {
            var list = _sqlSugarClient.Queryable<T_Entity>().Take(QUERY_TAKEN).ToList();
            return list;
        }

        public override int Update()
        {
            throw new NotImplementedException();
        }
    }
}
