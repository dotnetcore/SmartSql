﻿using System.Collections.Generic;
using SmartSql.Bulk;
using SmartSql.Bulk.MySql;
using SmartSql.DataSource;
using SmartSql.Test.Entities;

namespace SmartSql.Test.Unit.Bulk
{
    public class MySqlTest
    {
        [EnvironmentFact(include:"MY_SQL")]
        public void Insert()
        {
            var dbSessionFactory = new SmartSqlBuilder()
                .UseDataSource(DbProvider.MYSQL, "Data Source=localhost;database=SmartSqlTestDB;uid=root;pwd=root")
                .UseAlias("MySqlTest")
                .Build().GetDbSessionFactory();

            var list = new List<User> {
                new User {Id = 3, UserName = "1"}
                , new User {Id = 4, UserName = "2"}
            };
            using (var dbSession = dbSessionFactory.Open())
            {
                var data = list.ToDataTable();
                data.Columns.RemoveAt(0);
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
