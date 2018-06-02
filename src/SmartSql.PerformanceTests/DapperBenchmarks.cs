using BenchmarkDotNet.Attributes;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartSql.PerformanceTests
{

    public class DapperBenchmarks : BenchmarkBase
    {
        string _insertSql = @"
                        INSERT INTO T_Entity
                              (FString
                              ,FDecimal
                              ,FNullDecimal
                              ,FBool
                              ,FNullBool
                              ,Status
                              ,NullStatus
                              ,CreationTime
                              ,LastUpdateTime)
                              VALUES
                              (@FString
                              ,@FDecimal
                              ,@FNullDecimal
                              ,@FBool
                              ,@FNullBool
                              ,@Status
                              ,@NullStatus
                              ,@CreationTime
                              ,@LastUpdateTime)
                              ;Select Scope_Identity();";
        //[Benchmark]
        public override long Insert()
        {
            using (var conn = System.Data.SqlClient.SqlClientFactory.Instance.CreateConnection())
            {
                conn.ConnectionString = ConnectionString;
                return conn.ExecuteScalar<long>(_insertSql, GetInsertEntity());
            }
        }

        public  IEnumerable<T_Entity> Parser()
        {
            var list = new List<T_Entity>();
            WrapParser((dataReader) =>
            {
                if (dataReader.Read())
                {
                    var deser = SqlMapper.GetRowParser<T_Entity>(dataReader);
                    do
                    {
                        var obj = (T_Entity)deser.Invoke(dataReader);
                        list.Add(obj);
                    } while (dataReader.Read());
                }
                dataReader.Dispose();
            });
            return list;
        }

        [Benchmark]
        public override IEnumerable<T_Entity> Query()
        {
            using (var conn = System.Data.SqlClient.SqlClientFactory.Instance.CreateConnection())
            {
                conn.ConnectionString = ConnectionString;
                conn.Open();
                var list = conn.Query<T_Entity>(QueryString).ToList();
                return list;
            }
        }

        public override int Update()
        {
            throw new NotImplementedException();
        }
    }
}
