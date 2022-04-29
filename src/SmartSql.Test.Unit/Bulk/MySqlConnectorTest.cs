using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Bulk;
using SmartSql.DataSource;
using SmartSql.Test.Entities;
using Xunit;
using SmartSql.Bulk.MySqlConnector;

namespace SmartSql.Test.Unit.Bulk
{
    public class MySqlConnectorTest
    {
        [EnvironmentFact(include:"MY_SQL")]
        public void Insert()
        {
            var dbSessionFactory = new SmartSqlBuilder()
                .UseDataSource(DbProvider.MYSQL_CONNECTOR, "Data Source=localhost;database=SmartSqlTestDB;uid=root;pwd=root")
                .UseAlias("MySqlConnectorTest")
                .Build().GetDbSessionFactory();

            var list = new List<User> {
                new User {Id = 1, UserName = "1"}
                , new User {Id = 2, UserName = "2"}
            };
            using (var dbSession = dbSessionFactory.Open())
            {
                var data = list.ToDataTable();
                data.TableName = "t_user";
                BulkInsert bulkInsert = new BulkInsert(dbSession)
                {
                    SecureFilePriv = "C:/ProgramData/MySQL/MySQL Server 8.0/Uploads",
                    Table = data
                };
                bulkInsert.Insert();
            }
        }
    }
}
