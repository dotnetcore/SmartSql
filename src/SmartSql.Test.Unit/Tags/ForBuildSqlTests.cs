using System;
using System.Collections.Generic;
using FluentAssertions;
using SmartSql;
using SmartSql.Configuration;
using SmartSql.Configuration.Tags;
using SmartSql.Data;
using SmartSql.DataSource;
using SmartSql.Utils;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    public class ForBuildSqlTests
    {
        private static RequestContext CreateContext(
            string property,
            object value,
            out SmartSqlConfig config)
        {
            config = new SmartSqlConfig
            {
                Database = new Database
                {
                    DbProvider = new DbProvider
                    {
                        ParameterPrefix = "@",
                        Name = "MySql"
                    }
                },
                Settings = new SmartSql.Configuration.Settings
                {
                    IgnoreParameterCase = false
                }
            };
            config.SqlParamAnalyzer = new SqlParamAnalyzer(false, "@");

            var sqlParams = new SqlParameterCollection();
            sqlParams.TryAdd(property, value);
            var context = new RequestContext
            {
                Request = sqlParams,
                ExecutionContext = new ExecutionContext
                {
                    SmartSqlConfig = config
                }
            };
            context.SetupParameters();
            return context;
        }

        [Fact]
        public void Should_BuildInClause_When_DirectValueCollection()
        {
            var context = CreateContext("Ids", new List<int> { 1, 2, 3 }, out _);
            var forTag = new For
            {
                Property = "Ids",
                Key = "Id",
                Open = "(",
                Close = ")",
                Separator = ","
            };
            var childText = new SqlText("@Id", "@");
            forTag.ChildTags = new List<ITag> { childText };

            forTag.BuildSql(context);

            var sql = context.SqlBuilder.ToString();
            sql.Should().Contain("@Id_For__0");
            sql.Should().Contain("@Id_For__1");
            sql.Should().Contain("@Id_For__2");

            context.Parameters.TryGetValue("Id_For__0", out var p0).Should().BeTrue();
            p0.Value.Should().Be(1);
            context.Parameters.TryGetValue("Id_For__1", out var p1).Should().BeTrue();
            p1.Value.Should().Be(2);
            context.Parameters.TryGetValue("Id_For__2", out var p2).Should().BeTrue();
            p2.Value.Should().Be(3);
        }

        [Fact]
        public void Should_BuildSeparatedList_When_MultipleItems()
        {
            var context = CreateContext("Names", new List<string> { "Alice", "Bob" }, out _);
            var forTag = new For
            {
                Property = "Names",
                Key = "Name",
                Separator = ","
            };
            var childText = new SqlText("@Name", "@");
            forTag.ChildTags = new List<ITag> { childText };

            forTag.BuildSql(context);

            var sql = context.SqlBuilder.ToString();
            sql.Should().Contain("@Name_For__0");
            sql.Should().Contain("@Name_For__1");

            var separatorCount = CountOccurrences(sql, " , ");
            separatorCount.Should().Be(1);
        }

        [Fact]
        public void Should_WrapWithOpenClose_When_Specified()
        {
            var context = CreateContext("Ids", new List<int> { 10 }, out _);
            var forTag = new For
            {
                Property = "Ids",
                Key = "Id",
                Open = "(",
                Close = ")",
                Separator = ","
            };
            var childText = new SqlText("@Id", "@");
            forTag.ChildTags = new List<ITag> { childText };

            forTag.BuildSql(context);

            var sql = context.SqlBuilder.ToString();
            sql.Should().Contain("(");
            sql.Should().Contain(")");
        }

        [Fact]
        public void Should_SkipBuildSql_When_CollectionIsEmpty()
        {
            var context = CreateContext("Ids", new List<int>(), out _);
            var forTag = new For
            {
                Property = "Ids",
                Key = "Id",
                Open = "(",
                Close = ")",
                Separator = ","
            };
            var childText = new SqlText("@Id", "@");
            forTag.ChildTags = new List<ITag> { childText };

            forTag.BuildSql(context);

            context.SqlBuilder.ToString().Should().BeEmpty();
        }

        [Fact]
        public void Should_Throw_When_KeyIsEmpty()
        {
            var context = CreateContext("Ids", new List<int> { 1 }, out _);
            var forTag = new For
            {
                Property = "Ids",
                Key = "",
                Separator = ","
            };
            var childText = new SqlText("@Id", "@");
            forTag.ChildTags = new List<ITag> { childText };

            var act = () => forTag.BuildSql(context);

            act.Should().Throw<Exception>().WithMessage("*Key*required*");
        }

        [Fact]
        public void Should_Throw_When_NoChildTags()
        {
            var context = CreateContext("Ids", new List<int> { 1 }, out _);
            var forTag = new For
            {
                Property = "Ids",
                Key = "Id",
                Separator = ",",
                ChildTags = new List<ITag>()
            };

            var act = () => forTag.BuildSql(context);

            act.Should().Throw<Exception>().WithMessage("*childTag*");
        }

        [Fact]
        public void Should_BuildWithArray_When_ValuesAreInts()
        {
            var context = CreateContext("Ids", new[] { 5, 10 }, out _);
            var forTag = new For
            {
                Property = "Ids",
                Key = "Id",
                Open = "(",
                Close = ")",
                Separator = ","
            };
            var childText = new SqlText("@Id", "@");
            forTag.ChildTags = new List<ITag> { childText };

            forTag.BuildSql(context);

            var sql = context.SqlBuilder.ToString();
            sql.Should().Contain("(");
            sql.Should().Contain(")");
            sql.Should().Contain("@Id_For__0");
            sql.Should().Contain("@Id_For__1");

            context.Parameters.TryGetValue("Id_For__0", out var p0).Should().BeTrue();
            p0.Value.Should().Be(5);
            context.Parameters.TryGetValue("Id_For__1", out var p1).Should().BeTrue();
            p1.Value.Should().Be(10);
        }

        [Fact]
        public void Should_BuildWithCustomPrefix_When_DbProviderUsesDifferentPrefix()
        {
            var config = new SmartSqlConfig
            {
                Database = new Database
                {
                    DbProvider = new DbProvider
                    {
                        ParameterPrefix = "?",
                        Name = "PostgreSql"
                    }
                },
                Settings = new SmartSql.Configuration.Settings
                {
                    IgnoreParameterCase = false
                }
            };
            config.SqlParamAnalyzer = new SqlParamAnalyzer(false, "?");

            var sqlParams = new SqlParameterCollection();
            sqlParams.TryAdd("Ids", new List<int> { 1, 2 });
            var context = new RequestContext
            {
                Request = sqlParams,
                ExecutionContext = new ExecutionContext
                {
                    SmartSqlConfig = config
                }
            };
            context.SetupParameters();

            var forTag = new For
            {
                Property = "Ids",
                Key = "Id",
                Open = "(",
                Close = ")",
                Separator = ","
            };
            var childText = new SqlText("?Id", "?");
            forTag.ChildTags = new List<ITag> { childText };

            forTag.BuildSql(context);

            var sql = context.SqlBuilder.ToString();
            sql.Should().Contain("?Id_For__0");
            sql.Should().Contain("?Id_For__1");
        }

        [Fact]
        public void Should_HandleSingleItem_When_OnlyOneElement()
        {
            var context = CreateContext("Ids", new List<int> { 42 }, out _);
            var forTag = new For
            {
                Property = "Ids",
                Key = "Id",
                Open = "(",
                Close = ")",
                Separator = ","
            };
            var childText = new SqlText("@Id", "@");
            forTag.ChildTags = new List<ITag> { childText };

            forTag.BuildSql(context);

            var sql = context.SqlBuilder.ToString();
            sql.Should().Contain("@Id_For__0");
            sql.Should().NotContain("@Id_For__1");

            context.Parameters.TryGetValue("Id_For__0", out var p0).Should().BeTrue();
            p0.Value.Should().Be(42);
        }

        private static int CountOccurrences(string source, string substring)
        {
            int count = 0;
            int index = 0;
            while ((index = source.IndexOf(substring, index, StringComparison.Ordinal)) >= 0)
            {
                count++;
                index += substring.Length;
            }
            return count;
        }
    }
}
