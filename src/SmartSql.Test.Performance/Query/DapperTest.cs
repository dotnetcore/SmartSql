using BenchmarkDotNet.Attributes;
using Dapper;
using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SmartSql.Test.Performance.Query
{
    public class DapperTest : AbstractQueryTest
    {
        [GlobalSetup]
        public void Setup()
        {
            Query_1();
        }
        [BenchmarkCategory("Query", "Query_1")]
        [Benchmark(Baseline = true)]
        public AllPrimitive Query_1()
        {
            using (IDbConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                return connection.QueryFirstOrDefault<AllPrimitive>(QUERY_SQL, new { Taken = TAKEN_1 });
            }
        }
        [BenchmarkCategory("Query", "Query_10")]
        [Benchmark(Baseline = true)]
        public IList<AllPrimitive> Query_10()
        {
            using (IDbConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                return connection.Query<AllPrimitive>(QUERY_SQL, new { Taken = TAKEN_10 }).ToList();
            }
        }
        [BenchmarkCategory("Query", "Query_100")]
        [Benchmark(Baseline = true)]
        public IList<AllPrimitive> Query_100()
        {
            using (IDbConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                return connection.Query<AllPrimitive>(QUERY_SQL, new { Taken = TAKEN_100 }).ToList();
            }
        }
        [BenchmarkCategory("Query", "Query_1000")]
        [Benchmark(Baseline = true)]
        public IList<AllPrimitive> Query_1000()
        {
            using (IDbConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                return connection.Query<AllPrimitive>(QUERY_SQL, new { Taken = TAKEN_1000 }).ToList();
            }
        }
        [BenchmarkCategory("Query", "Query_1000")]
        [Benchmark()]
        public IList<dynamic> Query_Dynamic_1000()
        {
            using (IDbConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                return connection.Query<dynamic>(QUERY_SQL, new { Taken = TAKEN_1000 }).ToList();
            }
        }
    }
}
