using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Dapper;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SmartSql.Tests.Performance
{
    public class Dapper_Test
    {
        int testTime = 10000;
        [Fact]
        public void Query()
        {
            using (var conn = SqlClientFactory.Instance.CreateConnection())
            {
                conn.ConnectionString = "Data Source=.;database=TestDB;uid=sa;pwd=SmartSql.net";
                var list = conn.Query<T_Test>("SELECT Top 10 T.* From T_Test T With(NoLock) Where 1=1");
            }
        }

        [Fact]
        public void Querys_Scoped()
        {
            int i = 0;
            using (var conn = SqlClientFactory.Instance.CreateConnection())
            {
                conn.ConnectionString = "Data Source=.;database=TestDB;uid=sa;pwd=SmartSql.net";
                for (i = 0; i < testTime; i++)
                {
                    var list = conn.Query<T_Test>("      SELECT Top 10 T.* From T_Test T With(NoLock)            Where 1=1      ");
                }
            }
            Assert.Equal<int>(i, testTime);
        }
        [Fact]

        public void Querys_Transient()
        {
            int i = 0;
            for (i = 0; i < testTime; i++)
            {
                using (var conn = SqlClientFactory.Instance.CreateConnection())
                {
                    conn.ConnectionString = "Data Source=.;database=TestDB;uid=sa;pwd=SmartSql.net";
                    var list = conn.Query<T_Test>("Select Top 10 * From T_Test T Where 1=1  And 2=2 Order By Id Desc   ");

                }
            }
            Assert.Equal<int>(i, testTime);
        }
        [Fact]
        public async void QueryAsync()
        {
            //List<Task> tasks = new List<Task>();
            int i = 0;
            for (i = 0; i < testTime; i++)
            {
                using (var conn = SqlClientFactory.Instance.CreateConnection())
                {
                    conn.ConnectionString = "Data Source=.;database=TestDB;uid=sa;pwd=SmartSql.net";
                    //conn.Open();

                    var list = await conn.QueryAsync<T_Test>("Select Top 10 * From T_Test T Where 1=1  And 2=2 Order By Id Desc   ");
                    //tasks.Add(list);
                    //conn.Close();
                }
            }
            //Task.WaitAll(tasks.ToArray());
            Assert.Equal<int>(i, testTime);
        }
    }
}
