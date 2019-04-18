using BenchmarkDotNet.Attributes;
using SmartSql.DataSource;
using SmartSql.DbSession;
using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using SmartSql.DyRepository;
using SmartSql.Test.Repositories;

namespace SmartSql.Test.Performance.Query
{
    public class SmartSqlTest : AbstractQueryTest
    {
        public IDbSessionFactory DbSessionFactory { get; private set; }
        public WriteDataSource WriteDataSource { get; set; }
        private String Scope { get; set; }
        private IAllPrimitiveRepository Repository { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            Scope = nameof(AllPrimitive);
            var smartSqlBuilder = new SmartSqlBuilder().UseXmlConfig().UseCache(false).Build();
            DbSessionFactory = smartSqlBuilder.GetDbSessionFactory();
            var repositoryBuilder = new EmitRepositoryBuilder(null, null, Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance);
            var repositoryFactory = new RepositoryFactory(repositoryBuilder, Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance);
            Repository = repositoryFactory.CreateInstance(typeof(IAllPrimitiveRepository), smartSqlBuilder.SqlMapper) as IAllPrimitiveRepository;

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
        [BenchmarkCategory("Query", "Query_1")]
        [Benchmark]
        public IList<AllPrimitive> Query_DyRepository_1()
        {
            return Repository.Query(TAKEN_10) ;
        }
        [BenchmarkCategory("Query", "Query_1")]
        [Benchmark]
        public IList<AllPrimitive> Query_10()
        {
            using (var dbSession = DbSessionFactory.Open(WriteDataSource))
            {
                return dbSession.Query<AllPrimitive>(new RequestContext
                {
                    Scope = Scope,
                    SqlId = "Query",
                    Request = new { Taken = TAKEN_10 }
                }) as List<AllPrimitive>;
            }
        }
        [BenchmarkCategory("Query", "Query_10")]
        [Benchmark]
        public IList<AllPrimitive> Query_DyRepository_10()
        {
            return Repository.Query(TAKEN_10);
        }
        [BenchmarkCategory("Query", "Query_100")]
        [Benchmark]
        public IList<AllPrimitive> Query_100()
        {
            using (var dbSession = DbSessionFactory.Open(WriteDataSource))
            {
                return dbSession.Query<AllPrimitive>(new RequestContext
                {
                    Scope = Scope,
                    SqlId = "Query",
                    Request = new { Taken = TAKEN_100 }
                }) ;
            }
        }
        [BenchmarkCategory("Query", "Query_100")]
        [Benchmark]
        public IList<AllPrimitive> Query_DyRepository_100()
        {
            return Repository.Query(TAKEN_100);
        }
        [BenchmarkCategory("Query", "Query_1000")]
        [Benchmark]
        public IList<AllPrimitive> Query_1000()
        {
            using (var dbSession = DbSessionFactory.Open(WriteDataSource))
            {
                return dbSession.Query<AllPrimitive>(new RequestContext
                {
                    Scope = Scope,
                    SqlId = "Query",
                    Request = new { Taken = TAKEN_1000 }
                });
            }
        }
        [BenchmarkCategory("Query", "Query_1000")]
        [Benchmark]
        public IList<AllPrimitive> Query_DyRepository_1000()
        {
            return Repository.Query(TAKEN_1000);
        }
        [BenchmarkCategory("Query", "Query_1000")]
        [Benchmark]
        public IList<dynamic> Query_Dynamic_1000()
        {
            using (var dbSession = DbSessionFactory.Open(WriteDataSource))
            {
                return dbSession.Query<dynamic>(new RequestContext
                {
                    Scope = Scope,
                    SqlId = "Query",
                    Request = new { Taken = TAKEN_1000 }
                });
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
