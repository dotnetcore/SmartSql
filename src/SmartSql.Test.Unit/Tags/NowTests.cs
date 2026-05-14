using FluentAssertions;
using SmartSql.Configuration.Tags;
using SmartSql.Data;
using System;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    public class NowTests
    {
        private RequestContext CreateContext()
        {
            var sqlParams = new SqlParameterCollection();
            var context = new RequestContext
            {
                Request = sqlParams
            };
            context.SetupParameters();
            return context;
        }

        [Fact]
        public void Should_AlwaysReturnTrue_When_IsConditionCalled()
        {
            var tag = new Now { Property = "CreatedAt" };
            var context = CreateContext();

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_AddCurrentTimeToParameters_When_BuildSqlCalled()
        {
            var tag = new Now { Property = "CreatedAt" };
            var context = CreateContext();
            var before = DateTime.Now;

            tag.BuildSql(context);

            context.Parameters.TryGetValue("CreatedAt", out var param).Should().BeTrue();
            var paramValue = param.Value.Should().BeOfType<DateTime>().Subject;
            paramValue.Should().BeCloseTo(before, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void Should_AddUtcTimeToParameters_When_KindIsUtc()
        {
            var tag = new Now { Property = "CreatedAt", Kind = "UTC" };
            var context = CreateContext();
            var before = DateTime.UtcNow;

            tag.BuildSql(context);

            context.Parameters.TryGetValue("CreatedAt", out var param).Should().BeTrue();
            var paramValue = param.Value.Should().BeOfType<DateTime>().Subject;
            paramValue.Kind.Should().Be(DateTimeKind.Utc);
            paramValue.Should().BeCloseTo(before, TimeSpan.FromSeconds(5));
        }
    }
}
