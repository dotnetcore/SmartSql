using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using FluentAssertions;
using Moq;
using SmartSql.Configuration;
using SmartSql.Configuration.Tags;
using SmartSql.DataSource;
using Xunit;

namespace SmartSql.Test.Unit.Tags;

public class TagBuildSqlTests
{
    private class TestTag : Tag
    {
        private readonly bool _isCondition;

        public TestTag(bool isCondition = true, string prepend = null)
        {
            _isCondition = isCondition;
            Prepend = prepend;
        }

        public override bool IsCondition(AbstractRequestContext context)
        {
            return _isCondition;
        }
    }

    private static AbstractRequestContext CreateContext()
    {
        var config = new SmartSqlConfig
        {
            Database = new Database
            {
                DbProvider = new DbProvider { ParameterPrefix = "@" }
            }
        };

        var request = new RequestContext<object>
        {
            ExecutionContext = new ExecutionContext
            {
                SmartSqlConfig = config
            }
        };

        return request;
    }

    private static void SetIgnorePrepend(AbstractRequestContext context, bool value)
    {
        var prop = typeof(AbstractRequestContext).GetProperty("IgnorePrepend",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        prop.SetValue(context, value);
    }

    private static bool GetIgnorePrepend(AbstractRequestContext context)
    {
        var prop = typeof(AbstractRequestContext).GetProperty("IgnorePrepend",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        return (bool)prop.GetValue(context);
    }

    [Fact]
    public void Should_AppendPrepend_When_IgnorePrependIsFalse()
    {
        var tag = new TestTag(true, "AND");
        var context = CreateContext();

        tag.BuildSql(context);

        context.SqlBuilder.ToString().Should().Contain("AND");
    }

    [Fact]
    public void Should_SkipPrepend_When_IgnorePrependIsTrue()
    {
        var tag = new TestTag(true, "AND");
        var context = CreateContext();
        SetIgnorePrepend(context, true);

        tag.BuildSql(context);

        context.SqlBuilder.ToString().Should().NotContain("AND");
    }

    [Fact]
    public void Should_ResetIgnorePrepend_When_Skipped()
    {
        var tag = new TestTag(true, "AND");
        var context = CreateContext();
        SetIgnorePrepend(context, true);

        tag.BuildSql(context);

        GetIgnorePrepend(context).Should().BeFalse();
    }

    [Fact]
    public void Should_NotResetIgnorePrepend_When_NotIgnored()
    {
        var tag = new TestTag(true, "AND");
        var context = CreateContext();
        SetIgnorePrepend(context, false);

        tag.BuildSql(context);

        GetIgnorePrepend(context).Should().BeFalse();
    }

    [Fact]
    public void Should_SkipBuildSql_When_NotCondition()
    {
        var tag = new TestTag(false, "AND");
        var context = CreateContext();

        tag.BuildSql(context);

        context.SqlBuilder.ToString().Should().BeEmpty();
    }

    [Fact]
    public void Should_BuildChildSql_When_ChildTagsExist()
    {
        var childMock = new Mock<ITag>();
        childMock.Setup(c => c.IsCondition(It.IsAny<AbstractRequestContext>())).Returns(true);

        var tag = new TestTag(true, "AND");
        tag.ChildTags = new List<ITag> { childMock.Object };

        var context = CreateContext();

        tag.BuildSql(context);

        childMock.Verify(c => c.BuildSql(context), Times.Once);
    }

    [Fact]
    public void Should_NotBuildChildSql_When_ChildTagsAreNull()
    {
        var tag = new TestTag(true, "AND");
        tag.ChildTags = null;

        var context = CreateContext();

        var act = () => tag.BuildSql(context);

        act.Should().NotThrow();
    }

    [Fact]
    public void Should_NotBuildChildSql_When_ChildTagsAreEmpty()
    {
        var tag = new TestTag(true, "AND");
        tag.ChildTags = new List<ITag>();

        var context = CreateContext();

        tag.BuildSql(context);

        context.SqlBuilder.ToString().Should().Contain("AND");
    }

    [Fact]
    public void Should_BuildChildSql_When_MultipleChildTags()
    {
        var child1 = new Mock<ITag>();
        child1.Setup(c => c.IsCondition(It.IsAny<AbstractRequestContext>())).Returns(true);
        var child2 = new Mock<ITag>();
        child2.Setup(c => c.IsCondition(It.IsAny<AbstractRequestContext>())).Returns(true);

        var tag = new TestTag(true, "WHERE");
        tag.ChildTags = new List<ITag> { child1.Object, child2.Object };

        var context = CreateContext();

        tag.BuildSql(context);

        child1.Verify(c => c.BuildSql(context), Times.Once);
        child2.Verify(c => c.BuildSql(context), Times.Once);
    }

    [Fact]
    public void Should_NotBuildAny_When_NotConditionWithChildTags()
    {
        var childMock = new Mock<ITag>();
        childMock.Setup(c => c.IsCondition(It.IsAny<AbstractRequestContext>())).Returns(true);

        var tag = new TestTag(false, "AND");
        tag.ChildTags = new List<ITag> { childMock.Object };

        var context = CreateContext();

        tag.BuildSql(context);

        context.SqlBuilder.ToString().Should().BeEmpty();
        childMock.Verify(c => c.BuildSql(It.IsAny<AbstractRequestContext>()), Times.Never);
    }

    [Fact]
    public void Should_AppendSpaces_When_BuildSql()
    {
        var tag = new TestTag(true, "AND");
        var context = CreateContext();

        tag.BuildSql(context);

        var sql = context.SqlBuilder.ToString();
        sql.Should().StartWith(" ");
        sql.Should().Contain("AND");
    }
}
