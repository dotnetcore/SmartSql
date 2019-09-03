using System;
using System.Linq;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Unit.AutoConverter
{
    [Collection("GlobalSmartSql")]
    public class AutoConverterTest
    {
        protected ISqlMapper SqlMapper { get; }

        public AutoConverterTest(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
        }

        [Fact]
        public void DefaultAutoConverterTest()
        {
            var userId = SqlMapper.QuerySingle<long>(new RequestContext
            {
                Scope = "User",
                SqlId = "Insert",
                Request = new User
                {
                    UserName = "Noah",
                    Status = UserStatus.Ok
                }
            });
            
            var userEntity = SqlMapper.QuerySingle<User>(new RequestContext
            {
                Scope = "User",
                SqlId = "GetEntity",
                Request = new
                {
                    Id = userId
                }
            });
            
            Assert.NotNull(userEntity);
            Assert.Equal(userId, userEntity.Id);
        }

        [Fact]
        public void AssignAutoConverterTest()
        {
            var list = SqlMapper.Query<AutoConverter_2>(new RequestContext
            {
                Scope = "DefaultAutoConverter",
                SqlId = "Query",
                AutoConverterName = "DelimiterConverter"
            });
            
            Assert.NotNull(list);
            Assert.NotEqual(0, list.First().Col_Id);
        }
        
        [Fact]
        public void AssignFromMapAutoConverterTest()
        {
            var list = SqlMapper.Query<AutoConverter_2>(new RequestContext
            {
                Scope = "AssignAutoConverter",
                SqlId = "Query"
            });
            
            Assert.NotNull(list);
            Assert.NotEqual(0, list.First().Col_Id);
        }
    }
}