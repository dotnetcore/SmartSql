using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.DataAccess;
using Xunit;
using SmartSql.Abstractions;

namespace SmartSql.Tests.DataAccess
{

    public class DataAccess_Test : TestBase
    {
        private TestDataAccess dao;
        public DataAccess_Test()
        {
            dao = new TestDataAccess(SqlMapper);
        }

        [Fact]
        public void Insert()
        {
            var id = dao.Insert<long>(new T_Test { Name = "Dao-Insert" });

        }
        [Fact]
        public void Delete()
        {
            var exeNum = dao.Delete(230151);

        }

        [Fact]
        public void Update()
        {
            var exeNum = dao.Update(new T_Test
            {
                Id = 230150,
                Name = "Dao-Update"
            });

        }
        [Fact]
        public void GetList()
        {
            var list = dao.GetList(null);
            Assert.NotNull(list);
        }

        [Fact]
        public void GetListByPage()
        {
            var list = dao.GetListByPage(new { PageIndex = 1, PageSize = 10 });
            Assert.NotNull(list);
        }
        [Fact]
        public void GetRecord()
        {
            var record = dao.GetRecord(new { PageIndex = 1, PageSize = 10 });
            Assert.True(record > 0);
        }

        [Fact]
        public void GetEntity()
        {
            var entity = dao.GetEntity(240162);
            // Assert.NotNull(entity);
        }
    }

    public class TestDataAccess : DataAccessGeneric<T_Test>
    {
        public TestDataAccess(ISmartSqlMapper smartSqlMapper) : base(smartSqlMapper)
        {

        }
    }
}
