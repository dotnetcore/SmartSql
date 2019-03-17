using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SmartSql.Test.Unit.Deserializer
{
    public class EntityDeserializerTest : AbstractXmlConfigBuilderTest
    {


        [Fact]
        public void QuerySinge()
        {
            long id = Insert();
            var entity = DbSession.QuerySingle<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "GetEntity",
                Request = new { Id = id }
            });
            Assert.Equal(id, entity.Id);
        }

        private long Insert()
        {
            return DbSession.Insert<AllPrimitive, long>(new AllPrimitive
            {
                String = "Insert",
                DateTime = DateTime.Now
            });
        }

        [Fact]
        public void Query()
        {
            var list = DbSession.Query<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "Query",
                Request = new { Taken = 10000 }
            });
            list = DbSession.Query<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "Query",
                Request = new { Taken = 10000 }
            });
        }

        [Fact]
        public async Task QuerySingeAsync()
        {
            long id = Insert();
            var entity = await DbSession.QuerySingleAsync<AllPrimitive>(new RequestContext
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
            var list = await DbSession.QueryAsync<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "Query",
                Request = new { Taken = 10000 }
            });
            list = await DbSession.QueryAsync<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "Query",
                Request = new { Taken = 10000 }
            });
        }
    }
}
