using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace SmartSql.Test.Performance.Query
{
    [Config(typeof(QueryConfig))]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    [BenchmarkCategory("Query")]
    public abstract class AbstractQueryTest
    {
        public const String DbType = "SqlServer";
        public const String CONNECTION_STRING = "Data Source=.;Initial Catalog=SmartSqlTestDB;Integrated Security=True";
        public const int TAKEN_1 = 1;
        public const int TAKEN_10 = 10;
        public const int TAKEN_100 = 100;
        public const int TAKEN_1000 = 1000;
        public const String QUERY_SQL = @"SELECT Top (@Taken) T.* From T_AllPrimitive T With(NoLock)";
    }
}
