using SmartSql.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Dapper;
using System.Diagnostics;
using SmartSql.DataAccess;
using SmartSql.Exceptions;

namespace SmartSql.Tests
{
    public class SmartSqlMapper_Test : TestBase
    {
        [Fact]
        public void Insert()
        {
            int i = 0;
            int insertNum = 10;
            long preId = 0;
            for (i = 0; i < insertNum; i++)
            {
                preId = SqlMapper.ExecuteScalar<long>(new RequestContext
                {
                    Scope = "T_Test",
                    SqlId = "Insert",
                    Request = new T_Test { Name = $"Name-{preId}" }
                });
            }
            Assert.Equal<int>(i, insertNum);
        }
        [Fact]
        public void Delete()
        {
            var list = SqlMapper.Query<T_Test>(new RequestContext
            {
                Scope = "T_Test",
                SqlId = "GetList",
                Request = null
            });
            foreach (var test in list)
            {
                int exeNum = SqlMapper.Execute(new RequestContext
                {
                    Scope = "T_Test",
                    SqlId = "Delete",
                    Request = new { Id = test.Id }
                });
            }

        }
        [Fact]
        public void Update()
        {
            var list = SqlMapper.Query<T_Test>(new RequestContext
            {
                Scope = "T_Test",
                SqlId = "GetList",
                Request = null
            });

            foreach (var test in list)
            {
                test.Name = test.Name + "-Update";
                int exeNum = SqlMapper.Execute(new RequestContext
                {
                    Scope = "T_Test",
                    SqlId = "Update",
                    Request = test
                });
            }


        }

        [Fact]
        public void Query()
        {

            var list = SqlMapper.Query<T_Test>(new RequestContext
            {
                Scope = "T_Test",
                SqlId = "GetList",
                Request = new
                {
                    Ids = new int[] { 1, 2, 3, 45, 6 },
                    OrderBy = "4",
                    Id = 1,
                    Yes = true,
                    No = false
                    //Name="Hi"
                }
            });
            var list_c = SqlMapper.Query<T_Test>(new RequestContext
            {
                Scope = "T_Test",
                SqlId = "GetList",
                Request = new
                {
                    Ids = new int[] { 1, 2, 3, 45, 6 },
                    OrderBy = "4",
                    Id = 1,
                    Yes = true,
                    No = false
                    //Name="Hi"
                }
            });
            Assert.NotNull(list);
        }
        [Fact]
        public void Insert_Transaction()
        {
            var sqlMapper = MapperContainer.Instance.GetSqlMapper();
            try
            {
                sqlMapper.BeginTransaction();
                int i = 0;
                int insertNum = 10;
                long preId = 0;
                for (i = 0; i < insertNum; i++)
                {
                    preId = sqlMapper.ExecuteScalar<long>(new RequestContext
                    {
                        Scope = "T_Test",
                        SqlId = "Insert",
                        Request = new T_Test { Name = $"Name-{preId}" }
                    });
                }
                sqlMapper.CommitTransaction();
            }
            catch (Exception ex)
            {
                sqlMapper.RollbackTransaction();
                throw ex;
            }
        }

        [Fact]
        public void Update_Transaction()
        {
            try
            {
                SqlMapper.BeginTransaction();
                var list = SqlMapper.Query<T_Test>(new RequestContext
                {
                    Scope = "T_Test",
                    SqlId = "GetList",
                    Request = null
                });
                foreach (var test in list)
                {
                    test.Name = test.Name + "-Update";
                    int exeNum = SqlMapper.Execute(new RequestContext
                    {
                        Scope = "T_Test",
                        SqlId = "Update",
                        Request = test
                    });
                }

                SqlMapper.CommitTransaction();
            }
            catch (Exception ex)
            {
                SqlMapper.RollbackTransaction();
                throw ex;
            }
        }

        //[Fact]
        //public void StoredProcedure()
        //{
        //    using (var dbSession = SqlMapper.CreateDbSession(DataSourceChoice.Read))
        //    {
        //        var guid = dbSession.Connection.QuerySingle<Guid>(sql: "GetId", commandType: System.Data.CommandType.StoredProcedure);
        //        Assert.NotNull(guid);
        //    }
        //}
    }
}
