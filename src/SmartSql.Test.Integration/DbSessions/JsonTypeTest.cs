using SmartSql.IdGenerator;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Integration.DbSessions
{
    public class JsonTypeTest : IntegrationTestBase
    {
        public JsonTypeTest(SmartSqlFixture fixture) : base(fixture) { }

        [Fact]
        public void Insert()
        {
            var id = InsertImpl();
            Assert.NotEqual(0, id);
        }

        private long InsertImpl(UserExtendedInfo userExtendedInfo = null)
        {
            userExtendedInfo ??= NewUserExtendedInfo();
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
            var id = SnowflakeId.Default.NextId();
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
