using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Horology;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using SmartSql.PerformanceTests.Columns;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace SmartSql.PerformanceTests
{
    [OrderProvider(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    [Config(typeof(Config))]
    public abstract class BenchmarkBase
    {
        public const int ITERATIONS = 100;
        public const int QUERY_TAKEN = 40000;
        protected SqlConnection _connection;
        public static string ConnectionString { get; } = "Data Source=.;Initial Catalog=SmartSqlStarterDB;Integrated Security=True";
        public static string QueryString { get; } = $"SELECT TOP({QUERY_TAKEN}) T.* From T_Entity T With(NoLock)";

        public abstract long Insert();
        //public abstract int Delete();
        public abstract int Update();
        public abstract IEnumerable<T_Entity> Query();

        protected void WrapParser(Action<IDataReader> func)
        {
            var _dbConnection = SqlClientFactory.Instance.CreateConnection();
            _dbConnection.ConnectionString = ConnectionString;
            try
            {
                _dbConnection.Open();
                var cmd = _dbConnection.CreateCommand();
                cmd.CommandText = QueryString;
                var _dataReader = cmd.ExecuteReader();
                func(_dataReader);
            }
            finally
            {
                _dbConnection.Dispose();
            }
        }

        protected T_Entity GetInsertEntity()
        {
            return new T_Entity
            {
                CreationTime = DateTime.Now,
                FString = "SmartSql-" + this.GetHashCode(),
                FBool = true,
                FDecimal = 1,
                FLong = 1,
                FNullBool = false,
                FNullDecimal = 1,
                LastUpdateTime = null,
                NullStatus = EntityStatus.Ok,
                Status = EntityStatus.Ok
            };
        }
    }
    public class Config : ManualConfig
    {
        public Config()
        {
            Add(new MemoryDiagnoser());
            Add(new ORMColum());
            Add(new ReturnColum());
            Add(Job.Default
                //.WithUnrollFactor(BenchmarkBase.ITERATIONS)
                //.WithIterationTime(new TimeInterval(10000, TimeUnit.Millisecond))
                //.WithLaunchCount(1)
                //.WithWarmupCount(1)
                //.WithTargetCount(5)
                //.WithRemoveOutliers(true)
            );
        }
    }
}
