using System.Collections.Generic;
using SmartSql.Bulk;
using SmartSql.Bulk.MySqlConnector;
using SmartSql.DataSource;
using SmartSql.Test.Entities;
using Xunit;

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

            var list = new List<User>
            {
                new() { Id = 1, UserName = "jack" , IsDelete = true},
                new() { Id = 2, UserName = "lily", IsDelete = false}
            };
            using var dbSession = dbSessionFactory.Open();
            var data = list.ToDataTable();
            data.TableName = "t_user";
            var bulkInsert = new BulkInsert(dbSession)
            {
                SecureFilePriv = "C:/ProgramData/MySQL/MySQL Server 8.0/Uploads",
                Table = data
            };

            bulkInsert.Expressions.Add("user_name = upper(user_name)");
            bulkInsert.Expressions.Add("is_delete = convert(is_delete, unsigned )");
            bulkInsert.Insert();
        }
    }
}