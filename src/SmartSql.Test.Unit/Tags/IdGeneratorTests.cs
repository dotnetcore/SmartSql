using System;
using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using Moq;
using SmartSql.Configuration;
using SmartSql.Configuration.Tags;
using SmartSql.Data;
using SmartSql.DataSource;
using SmartSql.DbSession;
using SmartSql.IdGenerator;
using Xunit;
using IdGenTag = SmartSql.Configuration.Tags.IdGenerator;

namespace SmartSql.Test.Unit.Tags;

public class IdGeneratorTests
{
    private static IdGenTag CreateIdGeneratorTag(
        IIdGenerator idGenerator = null,
        string id = "TestId",
        bool assign = true)
    {
        return new IdGenTag
        {
            IdGen = idGenerator ?? new Mock<IIdGenerator>().Object,
            Id = id,
            Assign = assign,
            Statement = new Statement
            {
                SqlMap = new SqlMap
                {
                    SmartSqlConfig = new SmartSqlConfig
                    {
                        Database = new Database
                        {
                            DbProvider = new DbProvider { ParameterPrefix = "@" }
                        }
                    }
                }
            }
        };
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

        var request = new RequestContext<object>();
        request.ExecutionContext = new ExecutionContext
        {
            SmartSqlConfig = config
        };

        // Initialize Parameters via reflection since it has a protected setter
        var paramCollection = new SqlParameterCollection();
        var paramProp = typeof(AbstractRequestContext).GetProperty("Parameters",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        paramProp.SetValue(request, paramCollection);

        return request;
    }

    [Fact]
    public void Should_ReturnTrue_When_IsConditionCalled()
    {
        var mockIdGen = new Mock<IIdGenerator>();
        var tag = CreateIdGeneratorTag(mockIdGen.Object);

        var context = CreateContext();

        var result = tag.IsCondition(context);

        result.Should().BeTrue();
    }

    [Fact]
    public void Should_AlwaysReturnTrue_When_IsCondition()
    {
        var mockIdGen = new Mock<IIdGenerator>();
        var tag = CreateIdGeneratorTag(mockIdGen.Object);

        var context1 = CreateContext();
        var context2 = CreateContext();

        tag.IsCondition(context1).Should().BeTrue();
        tag.IsCondition(context2).Should().BeTrue();
    }

    [Fact]
    public void Should_CallNextId_When_BuildSqlCalled()
    {
        var mockIdGen = new Mock<IIdGenerator>();
        mockIdGen.Setup(g => g.NextId()).Returns(42);
        var tag = CreateIdGeneratorTag(mockIdGen.Object, "GeneratedId");

        var context = CreateContext();

        tag.BuildSql(context);

        mockIdGen.Verify(g => g.NextId(), Times.Once);
    }

    [Fact]
    public void Should_AddParameter_When_BuildSqlAddsNewId()
    {
        var mockIdGen = new Mock<IIdGenerator>();
        mockIdGen.Setup(g => g.NextId()).Returns(12345);
        var tag = CreateIdGeneratorTag(mockIdGen.Object, "NewId");

        var context = CreateContext();

        tag.BuildSql(context);

        context.Parameters.Should().ContainKey("NewId");
        context.Parameters["NewId"].Value.Should().Be(12345);
    }

    [Fact]
    public void Should_UpdateExistingParameter_When_IdAlreadyExists()
    {
        var mockIdGen = new Mock<IIdGenerator>();
        mockIdGen.Setup(g => g.NextId()).Returns(999);
        var tag = CreateIdGeneratorTag(mockIdGen.Object, "ExistingId", assign: false);

        var context = CreateContext();
        context.Parameters.TryAdd("ExistingId", new SqlParameter("ExistingId", 100));

        tag.BuildSql(context);

        context.Parameters["ExistingId"].Value.Should().Be(999);
    }

    [Fact]
    public void Should_NotAssignToSource_When_AssignIsFalse()
    {
        var mockIdGen = new Mock<IIdGenerator>();
        mockIdGen.Setup(g => g.NextId()).Returns(42);
        var tag = CreateIdGeneratorTag(mockIdGen.Object, "TestId", assign: false);

        var context = CreateContext();

        var act = () => tag.BuildSql(context);

        act.Should().NotThrow();
    }

    [Fact]
    public void Should_SetProperties_When_Created()
    {
        var mockIdGen = new Mock<IIdGenerator>();
        var tag = CreateIdGeneratorTag(mockIdGen.Object, "MyGeneratedId", assign: false);

        tag.Id.Should().Be("MyGeneratedId");
        tag.IdGen.Should().BeSameAs(mockIdGen.Object);
        tag.Assign.Should().BeFalse();
    }

    [Fact]
    public void Should_UseCustomIdGenerator_When_Provided()
    {
        var customIdGen = new CustomSnowflakeId();
        var tag = CreateIdGeneratorTag(customIdGen, "CustomId", assign: false);

        var context = CreateContext();

        tag.BuildSql(context);

        ((long)context.Parameters["CustomId"].Value).Should().BeGreaterThan(0);
    }

    [Fact]
    public void Should_GenerateMultipleUniqueIds_When_BuildSqlCalledMultipleTimes()
    {
        var mockIdGen = new Mock<IIdGenerator>();
        var callCount = 0;
        mockIdGen.Setup(g => g.NextId()).Returns(() => ++callCount);
        var tag = CreateIdGeneratorTag(mockIdGen.Object, "MultiId", assign: false);

        var context = CreateContext();

        tag.BuildSql(context);
        var id1 = (long)context.Parameters["MultiId"].Value;

        tag.BuildSql(context);
        var id2 = (long)context.Parameters["MultiId"].Value;

        id2.Should().BeGreaterThan(id1);
    }

    [Fact]
    public void Should_ThrowNullRef_When_IdGenIsNull()
    {
        var tag = new IdGenTag
        {
            IdGen = null,
            Id = "NullGenId",
            Assign = false,
            Statement = new Statement
            {
                SqlMap = new SqlMap
                {
                    SmartSqlConfig = new SmartSqlConfig
                    {
                        Database = new Database
                        {
                            DbProvider = new DbProvider { ParameterPrefix = "@" }
                        }
                    }
                }
            }
        };
        var context = CreateContext();

        var act = () => tag.BuildSql(context);

        act.Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void Should_SetStatement_When_Created()
    {
        var mockIdGen = new Mock<IIdGenerator>();
        var tag = CreateIdGeneratorTag(mockIdGen.Object);

        tag.Statement.Should().NotBeNull();
        tag.Statement.SqlMap.Should().NotBeNull();
        tag.Statement.SqlMap.SmartSqlConfig.Should().NotBeNull();
    }

    [Fact]
    public void Should_SetParent_When_Created()
    {
        var mockIdGen = new Mock<IIdGenerator>();
        var parentTag = new Mock<ITag>().Object;
        var tag = CreateIdGeneratorTag(mockIdGen.Object);
        tag.Parent = parentTag;

        tag.Parent.Should().BeSameAs(parentTag);
    }
}
