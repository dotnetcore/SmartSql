using SmartSql.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SmartSql.Tests.Performance
{
    public class SmartSqlMapper_Test : TestBase
    {
        const int testTime = 100000;
        [Fact]
        public void Query_Transient()
        {
            int i = 0;
            for (i = 0; i < testTime; i++)
            {
                var list = SqlMapper.Query<T_Test>(new RequestContext
                {
                    Scope = "T_Test",
                    SqlId = "GetList",

                });
            }
            Assert.Equal<int>(i, testTime);
        }
        [Fact]
        public void Query_LruCache()
        {
            int i = 0;
            for (i = 0; i < testTime; i++)
            {
                var list = SqlMapper.Query<T_Test>(new RequestContext
                {
                    Scope = "T_Test",
                    SqlId = "GetListByLruCache",

                });
            }
            Assert.Equal<int>(i, testTime);
        }
        [Fact]
        public void Query_RedisCache()
        {
            int i = 0;
            for (i = 0; i < testTime; i++)
            {
                var list = SqlMapper.Query<T_Test>(new RequestContext
                {
                    Scope = "T_Test",
                    SqlId = "GetListByRedisCache",

                });
            }
            Assert.Equal<int>(i, testTime);
        }

        [Fact]
        public void Query_Scoped()
        {
            int i = 0;
            SqlMapper.BeginSession();
            for (i = 0; i < testTime; i++)
            {
                var list = SqlMapper.Query<T_Test>(new RequestContext
                {
                    Scope = "T_Test",
                    SqlId = "GetList",

                });
            }
            SqlMapper.EndSession();
            Assert.Equal<int>(i, testTime);
        }

        [Fact]
        public async void QueryAsync()
        {
            int i = 0;
            //List<Task> tasks = new List<Task>();
           //SqlMapper.BeginSession();
            for (i = 0; i < testTime; i++)
            {
                var list =await SqlMapper.QueryAsync<T_Test>(new RequestContext
                {
                    Scope = "T_Test",
                    SqlId = "GetList",

                });
                //tasks.Add(list);
            }
            //Task.WaitAll(tasks.ToArray());
            //SqlMapper.EndSession();
            Assert.Equal<int>(i, testTime);
        }

    }
}
