using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SmartSql.Test.Unit.Deserializer
{
    [Collection("GlobalSmartSql")]
    public class EntityDeserializerTest 
    {
        protected ISqlMapper SqlMapper { get; }

        public EntityDeserializerTest(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
        }
        [Fact]
        public void QuerySingle()
        {
            long id = Insert();
            var entity = SqlMapper.QuerySingle<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "GetEntity",
                Request = new { Id = id }
            });
            Assert.Equal(id, entity.Id);
        }

        private long Insert()
        {
            return SqlMapper.Insert<AllPrimitive, long>(new AllPrimitive
            {
                String = "Insert",
                DateTime = DateTime.Now
            });
        }

        [Fact]
        public void Query()
        {
            var list = SqlMapper.Query<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "Query",
                Request = new { Taken = 10000 }
            });
            list = SqlMapper.Query<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "Query",
                Request = new { Taken = 10000 }
            });
        }

        [Fact]
        public async Task QuerySingleAsync()
        {
            long id = Insert();
            var entity = await SqlMapper.QuerySingleAsync<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "GetEntity",
                Request = new { Id = id }
            });
            Assert.Equal(id, entity.Id);
        }
        [Fact]
        public async Task QueryAsync()
        {
            var list = await SqlMapper.QueryAsync<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "Query",
                Request = new { Taken = 10000 }
            });
            list = await SqlMapper.QueryAsync<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "Query",
                Request = new { Taken = 10000 }
            });
        }
    }
}
