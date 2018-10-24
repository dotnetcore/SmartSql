using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace SmartSql.PerformanceTests
{
    public class EFContext : DbContext
    {
        public DbSet<T_Entity> Entities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(BenchmarkBase.ConnectionString);
        }
    }
    [Description("EF")]
    public class EFBenchmarks : BenchmarkBase
    {
        EFContext _efContext;
        [GlobalSetup]
        public void Setup()
        {
            _efContext = new EFContext();
        }
        [GlobalCleanup]
        public void Cleanup()
        {
            _efContext.Dispose();
        }
        public override long Insert()
        {

            throw new NotImplementedException();
        }

        [Benchmark]
        public override IEnumerable<T_Entity> Query()
        {
            return _efContext.Entities.Take(QUERY_TAKEN).ToList();
        }

        [Benchmark]
        public IEnumerable<T_Entity> SqlQuery()
        {
            return _efContext.Entities.FromSql(QueryString).ToList();
        }
        [Benchmark]
        public IEnumerable<T_Entity> SqlQuery_NoTracking()
        {
            return _efContext.Entities.AsNoTracking().FromSql(QueryString).ToList();
        }
        [Benchmark]
        public IEnumerable<T_Entity> Query_NoTracking()
        {
            return _efContext.Entities.AsNoTracking().Take(QUERY_TAKEN).ToList();
        }


        public override int Update()
        {
            throw new NotImplementedException();
        }
    }
}
