using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SmartSql.PerformanceTests
{
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [BenchmarkCategory("ORM")]
    public abstract class BenchmarkBase
    {
        public const int QUERY_TAKEN = 10000;
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
}
