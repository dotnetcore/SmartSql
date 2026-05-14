using FluentAssertions;
using SmartSql.Reflection;
using SmartSql.Reflection.Convert;
using SmartSql.Test.Unit.TestEntities;
using Xunit;

namespace SmartSql.Test.Unit.Reflection
{
    public class RequestConvertTests
    {
        [Fact]
        public void Should_ConvertToSqlParameters_When_UsingEntityObject()
        {
            var user = new User { Id = 0, UserName = "1" };

            var dic = RequestConvert.Instance.ToSqlParameters(user, true);
            var dic_1 = RequestConvert.Instance.ToSqlParameters(user, false);
            var dic_cache = RequestConvertCache<User, IgnoreCaseType>.Convert(user);
            var dic_cache_1 = RequestConvertCache<User>.Convert(user);

            dic.Should().NotBeNull();
            dic_1.Should().NotBeNull();
            dic_cache.Should().NotBeNull();
            dic_cache_1.Should().NotBeNull();
        }

        [Fact]
        public void Should_ConvertToSqlParameters_When_UsingAnonymousObject()
        {
            var dic = RequestConvert.Instance.ToSqlParameters(new { Id = 0, Name = 1 }, true);

            dic.Should().NotBeNull();
        }
    }
}
