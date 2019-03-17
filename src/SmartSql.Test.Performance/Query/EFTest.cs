using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace SmartSql.Test.Performance.Query
{
    public class EFContext : DbContext
    {
        public DbSet<AllPrimitive> Entities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(AbstracQueryTest.ConnectionString);
        }
    }
    public class EFTest : AbstracQueryTest
    {
        EFContext _dbContext;
        [GlobalSetup]
        public void Setup()
        {
            _dbContext = new EFContext();
            Query_1();
        }
        [BenchmarkCategory("Query", "Query_1")]
        [Benchmark]
        public AllPrimitive Query_1()
        {
            return _dbContext.Entities.AsNoTracking().Take(TAKEN_1).FirstOrDefault();
        }
        [BenchmarkCategory("Query", "Query_10")]
        [Benchmark]
        public List<AllPrimitive> Query_10()
        {
            return _dbContext.Entities.AsNoTracking().Take(TAKEN_10).ToList();
        }
        [BenchmarkCategory("Query", "Query_100")]
        [Benchmark]
        public List<AllPrimitive> Query_100()
        {
            return _dbContext.Entities.AsNoTracking().Take(TAKEN_100).ToList();
        }
        [BenchmarkCategory("Query", "Query_1000")]
        [Benchmark]
        public List<AllPrimitive> Query_1000()
        {
            return _dbContext.Entities.AsNoTracking().Take(TAKEN_1000).ToList();
        }
    }
}
