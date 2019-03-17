using BenchmarkDotNet.Attributes;
using SmartSql.DataSource;
using SmartSql.DbSession;
using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartSql.Test.Performance.Query
{
    public class SmartSqlTest : AbstracQueryTest
    {
        public IDbSessionFactory DbSessionFactory { get; private set; }
        public WriteDataSource WriteDataSource { get; set; }
        private String Scope { get; set; }
        [GlobalSetup]
        public void Setup()
        {
            Scope = nameof(AllPrimitive);
            DbSessionFactory = SmartSqlBuilder.AddXmlConfig().UseCache(false).Build().GetDbSessionFactory();
            WriteDataSource = DbSessionFactory.SmartSqlConfig.Database.Write;
            Query_1();
        }
        [BenchmarkCategory("Query", "Query_1")]
        [Benchmark]
        public AllPrimitive Query_1()
        {
            using (var dbSession = DbSessionFactory.Open(WriteDataSource))
            {
                return dbSession.QuerySingle<AllPrimitive>(new RequestContext
                {
                    Scope = Scope,
                    SqlId = "Query",
                    Request = new { Taken = TAKEN_1 }
                });
            }
        }
        [BenchmarkCategory("Query", "Query_10")]
        [Benchmark]
        public List<AllPrimitive> Query_10()
        {
            using (var dbSession = DbSessionFactory.Open(WriteDataSource))
            {
                return dbSession.Query<AllPrimitive>(new RequestContext
                {
                    Scope = Scope,
                    SqlId = "Query",
                    Request = new { Taken = TAKEN_10 }
                }).ToList();
            }
        }
        [BenchmarkCategory("Query", "Query_100")]
        [Benchmark]
        public List<AllPrimitive> Query_100()
        {
            using (var dbSession = DbSessionFactory.Open(WriteDataSource))
            {
                return dbSession.Query<AllPrimitive>(new RequestContext
                {
                    Scope = Scope,
                    SqlId = "Query",
                    Request = new { Taken = TAKEN_100 }
                }).ToList();
            }
        }
        [BenchmarkCategory("Query", "Query_1000")]
        [Benchmark]
        public List<AllPrimitive> Query_1000()
        {
            using (var dbSession = DbSessionFactory.Open(WriteDataSource))
            {
                return dbSession.Query<AllPrimitive>(new RequestContext
                {
                    Scope = Scope,
                    SqlId = "Query",
                    Request = new { Taken = TAKEN_1000 }
                }).ToList();
            }
        }
        [BenchmarkCategory("Query", "Query_1000")]
        [Benchmark]
        public List<dynamic> Query_Dynamic_1000()
        {
            using (var dbSession = DbSessionFactory.Open(WriteDataSource))
            {
                return dbSession.Query<dynamic>(new RequestContext
                {
                    Scope = Scope,
                    SqlId = "Query",
                    Request = new { Taken = TAKEN_1000 }
                }).ToList();
            }
        }
        //[BenchmarkCategory("Query", "Query_1000")]
        //[Benchmark]
        //public List<AllPrimitive> Query_FromSql_1000()
        //{
        //    using (var dbSession = DbSessionFactory.Open(WriteDataSource))
        //    {
        //        return dbSession.Query<AllPrimitive>(new RequestContext
        //        {
        //            RealSql = QUERY_SQL,
        //            Request = new { Taken = TAKEN_1000 }
        //        }).ToList();
        //    }
        //}
        //[BenchmarkCategory("Query")]
        //[Benchmark]
        //public List<AllPrimitive> Query_Parameters()
        //{
        //    using (var dbSession = DbSessionFactory.Open(WriteDataSource))
        //    {
        //        return dbSession.Query<AllPrimitive>(new RequestContext
        //        {
        //            Scope = Scope,
        //            SqlId = "Query",
        //            Request = QueryParamters
        //        }).ToList();
        //    }
        //}
        //[BenchmarkCategory("Query")]
        //[Benchmark]
        //public List<AllPrimitive> Query_FromSql_Parameters()
        //{
        //    using (var dbSession = DbSessionFactory.Open(WriteDataSource))
        //    {
        //        return dbSession.Query<AllPrimitive>(new RequestContext
        //        {
        //            RealSql = QUERY_SQL,
        //            Request = QueryParamters
        //        }).ToList();
        //    }
        //}
    }
}
