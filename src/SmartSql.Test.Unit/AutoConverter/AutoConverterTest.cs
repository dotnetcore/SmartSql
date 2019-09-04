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
            var userList = SqlMapper.Query<User>(new RequestContext
            {
                Scope = "User",
                SqlId = "Query"
            });
            
            Assert.NotNull(userList);
            Assert.NotEqual(0,userList.First().Id);
        }

        [Fact]
        public void DisabledAutoConverterTest()
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
                Scope = "DisabledAutoConverter",
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
        public void AssignAutoFromRequestConverterTest()
        {
            var list = SqlMapper.Query<AutoConverter_2>(new RequestContext
            {
                Scope = "DefaultAutoConverter",
                SqlId = "AssignAutoFromRequestConverterQuery",
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
        
        [Fact]
        public void AssignFromStatementAutoConverterTest()
        {
            var list = SqlMapper.Query<AutoConverter_1>(new RequestContext
            {
                Scope = "AssignAutoConverter",
                SqlId = "AssignAutoConverterQuery"
            });
            
            Assert.NotNull(list);
            Assert.NotEqual(0, list.First().Id);
            Assert.NotNull(list.First().Name);
        }

        /// <summary>
        /// 同时指定AutoConverter和ResultMap时，ResultMap优先级高于AutoConverter
        /// </summary>
        [Fact]
        public void AssignResultMapAndAutoConverterTest()
        {
            var list = SqlMapper.Query<AutoConverter_2>(new RequestContext
            {
                Scope = "AssignAutoConverter",
                SqlId = "AssignResultMapAndAutoConverterQuery"
            });
            
            Assert.NotNull(list);
            Assert.NotEqual(0, list.First().Col_Id);
            Assert.NotNull(list.First().Col_Name);
            
        }
    }
}