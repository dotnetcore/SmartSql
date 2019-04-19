using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit.DbSessions
{
    [Collection("GlobalSmartSql")]
    public class JsonTypeTest
    {
        protected ISqlMapper SqlMapper { get; }

        public JsonTypeTest(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
        }
        [Fact]
        public void Insert()
        {
            var id = InsertImpl();
            Assert.NotEqual(0, id);
        }

        private long InsertImpl(UserExtendedInfo userExtendedInfo = null)
        {
            if (userExtendedInfo == null)
            {
                userExtendedInfo = NewUserExtendedInfo();
            }
            SqlMapper.Execute(new RequestContext
            {
                Scope = nameof(UserExtendedInfo),
                SqlId = "Insert",
                Request = userExtendedInfo
            });
            return userExtendedInfo.UserId;
        }

        private UserExtendedInfo NewUserExtendedInfo()
        {
            SqlMapper.SmartSqlConfig.IdGenerators.TryGetValue("DbSequence", out var idGenerator);
            var id = idGenerator.NextId();
            return new UserExtendedInfo
            {
                UserId = id,
                Data = new UserInfo
                {
                    Height = 188,
                    Weight = 168
                }
            };
        }

        [Fact]
        public void GetById()
        {
            var insertUserExtendedInfo = NewUserExtendedInfo();
            var userId = InsertImpl(insertUserExtendedInfo);
            var userExtendedInfo = SqlMapper.QuerySingle<UserExtendedInfo>(new RequestContext
            {
                Scope = nameof(UserExtendedInfo),
                SqlId = "GetEntity",
                Request = new { UserId = userId }
            });
            Assert.Equal(userId, insertUserExtendedInfo.UserId);
            Assert.Equal(userExtendedInfo.Data.Height, userExtendedInfo.Data.Height);
            Assert.Equal(userExtendedInfo.Data.Weight, userExtendedInfo.Data.Weight);
        }
    }
}
