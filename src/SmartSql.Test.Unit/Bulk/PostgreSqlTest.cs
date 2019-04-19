using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Bulk;
using SmartSql.Bulk.PostgreSql;
using SmartSql.DataSource;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Unit.Bulk
{
    public class PostgreSqlTest
    {
        [Fact]
        public void Insert()
        {
            var dbSessionFactory = new SmartSqlBuilder()
                .UseDataSource(DbProvider.POSTGRESQL, "Server=localhost;Database=SmartSqlTestDB;Port=5432;User Id=postgres;Password=SmartSql.net;")
                .UseAlias("PostgreSqlTest")
                .Build().GetDbSessionFactory();

            var list = new List<User> {
                new User {Id = 1, UserName = "1"}
                , new User {Id = 2, UserName = "2"}
            };
            using (var dbSession = dbSessionFactory.Open())
            {
                var data = list.ToDataTable();
                data.Columns.RemoveAt(0);
                data.Columns["UserName"].ColumnName = "user_name";
                data.Columns["Status"].ColumnName = "status";
                data.TableName = "t_user";
                BulkInsert bulkInsert = new BulkInsert(dbSession);
                bulkInsert.Table = data;
                bulkInsert.Insert();
            }
        }
    }
}
