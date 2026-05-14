using FluentAssertions;
using SmartSql.Configuration;
using SmartSql.Configuration.Tags;
using SmartSql.Data;
using SmartSql.Exceptions;
using SmartSql.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using SmartSql.DataSource;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    public class ForBuildSqlDeepTests
    {
        #region Helper

        private RequestContext CreateContext(string property, object value)
        {
            var sqlParams = new SqlParameterCollection();
            sqlParams.TryAdd(property, value);
            var context = new RequestContext();
            // Set Parameters via reflection since protected set
            var prop = typeof(AbstractRequestContext).GetProperty("Parameters",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            prop.SetValue(context, sqlParams);
            context.ExecutionContext = new SmartSql.ExecutionContext
            {
                SmartSqlConfig = new SmartSqlConfig
                {
                    Database = new Database
                    {
                        DbProvider = new DbProvider
                        {
                            ParameterPrefix = "@"
                        }
                    },
                    SqlParamAnalyzer = new SqlParamAnalyzer(false, "@")
                }
            };
            // SqlBuilder has internal set, use reflection
            var sqlBuilderProp = typeof(AbstractRequestContext).GetProperty("SqlBuilder",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            sqlBuilderProp.SetValue(context, new System.Text.StringBuilder());
            return context;
        }

        private For CreateDefaultFor()
        {
            return new For
            {
                Open = "(",
                Separator = ",",
                Close = ")",
                Key = "Id",
                ChildTags = new List<ITag>
                {
                    new SqlText("@Id", "@")
                }
            };
        }

        #endregion

        [Fact]
        public void Should_ReturnTrue_When_CollectionIsNonEmpty()
        {
            var forTag = new For { Property = "Ids", Key = "Id" };
            var context = CreateContext("Ids", new List<int> { 1, 2, 3 });

            var result = forTag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_CollectionIsEmpty()
        {
            var forTag = new For { Property = "Ids", Key = "Id" };
            var context = CreateContext("Ids", new List<int>());

            var result = forTag.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnFalse_When_PropertyIsNull()
        {
            var forTag = new For { Property = "Ids", Key = "Id" };
            var context = CreateContext("Ids", null);

            var result = forTag.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnFalse_When_PropertyIsNotEnumerable()
        {
            var forTag = new For { Property = "Id", Key = "Id" };
            var context = CreateContext("Id", 42);

            var result = forTag.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnTrue_When_ArrayIsNonEmpty()
        {
            var forTag = new For { Property = "Ids", Key = "Id" };
            var context = CreateContext("Ids", new[] { 1, 2, 3 });

            var result = forTag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_ArrayIsEmpty()
        {
            var forTag = new For { Property = "Ids", Key = "Id" };
            var context = CreateContext("Ids", new int[0]);

            var result = forTag.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnFalse_When_PropertyIsMissing()
        {
            var forTag = new For { Property = "Ids", Key = "Id", Required = false };
            var sqlParams = new SqlParameterCollection();
            var context = new RequestContext();
            var prop = typeof(AbstractRequestContext).GetProperty("Parameters",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            prop.SetValue(context, sqlParams);
            context.ExecutionContext = new SmartSql.ExecutionContext
            {
                SmartSqlConfig = new SmartSqlConfig
                {
                    Database = new Database { DbProvider = new DbProvider { ParameterPrefix = "@" } },
                    SqlParamAnalyzer = new SqlParamAnalyzer(false, "@")
                }
            };
            // SqlBuilder has internal set, use reflection
            var sqlBuilderProp = typeof(AbstractRequestContext).GetProperty("SqlBuilder",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            sqlBuilderProp.SetValue(context, new System.Text.StringBuilder());

            var result = forTag.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_BuildSql_When_CollectionHasIntegers()
        {
            var forTag = CreateDefaultFor();
            forTag.Property = "Ids";
            var context = CreateContext("Ids", new List<int> { 1, 2, 3 });

            forTag.BuildSql(context);

            var sql = context.SqlBuilder.ToString();
            sql.Should().Contain("(");
            sql.Should().Contain(")");
            sql.Should().Contain("@Id_For__0");
            sql.Should().Contain("@Id_For__1");
            sql.Should().Contain("@Id_For__2");
        }

        [Fact]
        public void Should_BuildSql_When_CollectionHasStrings()
        {
            var forTag = CreateDefaultFor();
            forTag.Property = "Names";
            var context = CreateContext("Names", new List<string> { "Alice", "Bob" });

            forTag.BuildSql(context);

            context.SqlBuilder.ToString().Should().Contain("@Id_For__0");
            context.SqlBuilder.ToString().Should().Contain("@Id_For__1");
            context.Parameters.TryGetValue("Id_For__0", out var param0).Should().BeTrue();
            param0.Value.Should().Be("Alice");
        }

        [Fact]
        public void Should_BuildSql_When_CollectionHasArrays()
        {
            var forTag = CreateDefaultFor();
            forTag.Property = "Ids";
            var context = CreateContext("Ids", new[] { 10, 20, 30 });

            forTag.BuildSql(context);

            context.SqlBuilder.ToString().Should().Contain("@Id_For__0");
            context.SqlBuilder.ToString().Should().Contain("@Id_For__1");
            context.SqlBuilder.ToString().Should().Contain("@Id_For__2");
        }

        [Fact]
        public void Should_BuildSqlWithCustomSeparator_When_Specified()
        {
            var forTag = new For
            {
                Open = "{",
                Separator = "|",
                Close = "}",
                Key = "Id",
                Property = "Ids",
                ChildTags = new List<ITag>
                {
                    new SqlText("@Id", "@")
                }
            };
            var context = CreateContext("Ids", new List<int> { 1, 2 });

            forTag.BuildSql(context);

            var sql = context.SqlBuilder.ToString();
            sql.Should().Contain("{");
            sql.Should().Contain("}");
            sql.Should().Contain("|");
        }

        [Fact]
        public void Should_NotAddSeparator_When_SingleItem()
        {
            var forTag = CreateDefaultFor();
            forTag.Property = "Ids";
            var context = CreateContext("Ids", new List<int> { 42 });

            forTag.BuildSql(context);

            var sql = context.SqlBuilder.ToString();
            sql.Should().NotContain(",");
        }

        [Fact]
        public void Should_BuildSqlWithNonDirectValue_When_CollectionOfObjects()
        {
            var forTag = new For
            {
                Open = "(",
                Separator = ",",
                Close = ")",
                Key = "Item",
                Property = "Users",
                ChildTags = new List<ITag>
                {
                    new SqlText("@Item.Id,@Item.Name", "@")
                }
            };
            var users = new List<UserDto>
            {
                new UserDto { Id = 1, Name = "Alice" },
                new UserDto { Id = 2, Name = "Bob" }
            };
            var context = CreateContext("Users", users);

            forTag.BuildSql(context);

            context.SqlBuilder.ToString().Should().Contain("@Item_For__Id_0");
            context.SqlBuilder.ToString().Should().Contain("@Item_For__Name_0");
            context.SqlBuilder.ToString().Should().Contain("@Item_For__Id_1");
            context.SqlBuilder.ToString().Should().Contain("@Item_For__Name_1");
        }

        [Fact]
        public void Should_CreateParametersWithCorrectValues_When_SimpleCollection()
        {
            var forTag = CreateDefaultFor();
            forTag.Property = "Ids";
            var context = CreateContext("Ids", new List<int> { 100, 200, 300 });

            forTag.BuildSql(context);

            context.Parameters.TryGetValue("Id_For__0", out var p0).Should().BeTrue();
            p0.Value.Should().Be(100);

            context.Parameters.TryGetValue("Id_For__1", out var p1).Should().BeTrue();
            p1.Value.Should().Be(200);

            context.Parameters.TryGetValue("Id_For__2", out var p2).Should().BeTrue();
            p2.Value.Should().Be(300);
        }

        [Fact]
        public void Should_Throw_When_KeyIsNull()
        {
            var forTag = new For
            {
                Open = "(",
                Close = ")",
                Key = null,
                Property = "Ids",
                ChildTags = new List<ITag>
                {
                    new SqlText("@Id", "@")
                }
            };
            var context = CreateContext("Ids", new List<int> { 1 });

            var act = () => forTag.BuildSql(context);

            act.Should().Throw<SmartSqlException>()
                .WithMessage("*Key*required*");
        }

        [Fact]
        public void Should_Throw_When_ChildTagsAreEmpty()
        {
            var forTag = new For
            {
                Open = "(",
                Close = ")",
                Key = "Id",
                Property = "Ids",
                ChildTags = new List<ITag>()
            };
            var context = CreateContext("Ids", new List<int> { 1 });

            var act = () => forTag.BuildSql(context);

            act.Should().Throw<SmartSqlException>()
                .WithMessage("*must have childTag*");
        }

        [Fact]
        public void Should_Throw_When_ChildTagIsNotSqlText()
        {
            var forTag = new For
            {
                Open = "(",
                Close = ")",
                Key = "Id",
                Property = "Ids",
                ChildTags = new List<ITag>
                {
                    new IsNotEmpty { Property = "Name" }
                }
            };
            var context = CreateContext("Ids", new List<int> { 1 });

            var act = () => forTag.BuildSql(context);

            act.Should().Throw<SmartSqlException>()
                .WithMessage("*SqlText*");
        }

        [Fact]
        public void Should_CreateCorrectParameters_When_CollectionHasNullableInts()
        {
            var forTag = CreateDefaultFor();
            forTag.Property = "Ids";
            var context = CreateContext("Ids", new List<int?> { 1, null, 3 });

            forTag.BuildSql(context);

            context.Parameters.TryGetValue("Id_For__0", out var p0).Should().BeTrue();
            p0.Value.Should().Be(1);

            context.Parameters.TryGetValue("Id_For__1", out var p1).Should().BeTrue();
            p1.Value.Should().BeNull();

            context.Parameters.TryGetValue("Id_For__2", out var p2).Should().BeTrue();
            p2.Value.Should().Be(3);
        }

        [Fact]
        public void Should_BuildSqlWithSeparator_When_MultipleItems()
        {
            var forTag = CreateDefaultFor();
            forTag.Property = "Ids";
            forTag.Separator = ",";
            var context = CreateContext("Ids", new List<int> { 1, 2 });

            forTag.BuildSql(context);

            var sql = context.SqlBuilder.ToString();
            sql.Should().Contain(",");
        }

        public class UserDto
        {
            public long Id { get; set; }
            public string Name { get; set; }
        }
    }
}
