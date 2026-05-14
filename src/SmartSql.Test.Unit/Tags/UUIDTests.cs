using FluentAssertions;
using SmartSql.Configuration.Tags;
using SmartSql.Data;
using System;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    public class UUIDTests
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
            var tag = new UUID { Property = "Id" };
            var context = CreateContext();

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_AddGuidToParameters_When_BuildSqlCalledWithoutFormat()
        {
            var tag = new UUID { Property = "Id" };
            var context = CreateContext();

            tag.BuildSql(context);

            context.Parameters.TryGetValue("Id", out var param).Should().BeTrue();
            param.Value.Should().BeOfType<Guid>();
        }

        [Fact]
        public void Should_AddFormattedStringToParameters_When_BuildSqlCalledWithFormat()
        {
            var tag = new UUID { Property = "Id", Format = "N" };
            var context = CreateContext();

            tag.BuildSql(context);

            context.Parameters.TryGetValue("Id", out var param).Should().BeTrue();
            param.Value.Should().BeOfType<string>();
            var strVal = (string)param.Value;
            strVal.Should().HaveLength(32);
            strVal.Should().NotContain("-");
        }

        [Fact]
        public void Should_AddDashFormattedStringToParameters_When_BuildSqlCalledWithDFormat()
        {
            var tag = new UUID { Property = "Id", Format = "D" };
            var context = CreateContext();

            tag.BuildSql(context);

            context.Parameters.TryGetValue("Id", out var param).Should().BeTrue();
            param.Value.Should().BeOfType<string>();
            var strVal = (string)param.Value;
            strVal.Should().Contain("-");
        }

        [Fact]
        public void Should_GenerateDifferentGuids_When_BuildSqlCalledMultipleTimes()
        {
            var tag = new UUID { Property = "Id" };
            var context1 = CreateContext();
            var context2 = CreateContext();

            tag.BuildSql(context1);
            tag.BuildSql(context2);

            var id1 = context1.Parameters["Id"].Value;
            var id2 = context2.Parameters["Id"].Value;
            id1.Should().NotBe(id2);
        }

        [Fact]
        public void Should_SetProperty_When_Created()
        {
            var tag = new UUID { Property = "UserId" };

            tag.Property.Should().Be("UserId");
        }

        [Fact]
        public void Should_HaveNullFormat_When_CreatedWithoutFormat()
        {
            var tag = new UUID { Property = "Id" };

            tag.Format.Should().BeNull();
        }
    }
}
