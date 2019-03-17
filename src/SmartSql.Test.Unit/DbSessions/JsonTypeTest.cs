using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit.DbSessions
{
    public class JsonTypeTest : AbstractXmlConfigBuilderTest
    {
        [Fact]
        public void Insert()
        {
            var id = InsertImpl();
            Assert.NotEqual(0, id);
        }

        private long InsertImpl(User user = null)
        {
            if (user == null)
            {
                user = new User
                {
                    UserName = "SmartSql",
                    Info = new UserInfo
                    {
                        Height = 188,
                        Weight = 168
                    }
                };
            }
            return DbSession.ExecuteScalar<long>(new RequestContext
            {
                Scope = "User",
                SqlId = "Insert",
                Request = user
            });
        }

        [Fact]
        public void GetById()
        {
            var insertUser = new User
            {
                UserName = "SmartSql",
                Info = new UserInfo
                {
                    Height = 188,
                    Weight = 168
                }
            };
            var id = InsertImpl(insertUser);
            var user = DbSession.QuerySingle<User>(new RequestContext
            {
                Scope = "User",
                SqlId = "GetById",
                Request = new { Id = id }
            });
            Assert.Equal(id, user.Id);
            Assert.Equal(insertUser.UserName, user.UserName);
            Assert.Equal(insertUser.Info.Height, user.Info.Height);
            Assert.Equal(insertUser.Info.Weight, user.Info.Weight);
        }
    }
}
