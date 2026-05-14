using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using FluentAssertions;
using SmartSql.Configuration;
using SmartSql.Configuration.Tags;
using SmartSql.Configuration.Tags.TagBuilders;
using SmartSql.DataSource;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    public class TagBuilderAdditionalTests
    {
        private static XmlDocument CreateXmlDocument(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc;
        }

        private static Statement CreateStatement(string scope = "TestScope")
        {
            var statement = new Statement
            {
                SqlMap = new SqlMap
                {
                    Scope = scope,
                    SmartSqlConfig = new SmartSqlConfig
                    {
                        Database = new Database
                        {
                            DbProvider = new DbProvider { ParameterPrefix = "@" }
                        }
                    }
                }
            };
            // IncludeDependencies is internal; initialize via reflection so IncludeBuilder doesn't NRE
            var prop = typeof(Statement).GetProperty("IncludeDependencies",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            prop?.SetValue(statement, new List<Include>());
            return statement;
        }

        #region SetBuilder

        [Fact]
        public void Should_BuildSetTag_When_SetBuilderCalled()
        {
            var xml = "<Set />";
            var doc = CreateXmlDocument(xml);
            var statement = CreateStatement();
            var builder = new SetBuilder();

            var result = builder.Build(doc.DocumentElement, statement);

            result.Should().BeOfType<Set>();
            var set = result as Set;
            set.Prepend.Should().Be("Set");
            set.Required.Should().BeFalse();
            set.Min.Should().Be(SetBuilder.DEFAULT_MIN);
            set.Statement.Should().BeSameAs(statement);
        }

        [Fact]
        public void Should_HaveChildTagsList_When_SetBuilderCreatesTag()
        {
            var xml = "<Set Required=\"true\" />";
            var doc = CreateXmlDocument(xml);
            var statement = CreateStatement();
            var builder = new SetBuilder();

            var result = builder.Build(doc.DocumentElement, statement);

            var set = result as Set;
            set.ChildTags.Should().NotBeNull();
            set.ChildTags.Should().BeEmpty();
            set.Required.Should().BeTrue();
        }

        [Fact]
        public void Should_SetCustomMin_When_MinAttributeProvided()
        {
            var xml = "<Set Min=\"3\" />";
            var doc = CreateXmlDocument(xml);
            var statement = CreateStatement();
            var builder = new SetBuilder();

            var result = builder.Build(doc.DocumentElement, statement);

            var set = result as Set;
            set.Min.Should().Be(3);
        }

        [Fact]
        public void Should_UseDefaultMin_When_MinAttributeNotProvided()
        {
            var xml = "<Set />";
            var doc = CreateXmlDocument(xml);
            var statement = CreateStatement();
            var builder = new SetBuilder();

            var result = builder.Build(doc.DocumentElement, statement);

            var set = result as Set;
            set.Min.Should().Be(1);
        }

        #endregion

        #region IncludeBuilder

        [Fact]
        public void Should_BuildIncludeTag_When_IncludeBuilderCalled()
        {
            var xml = "<Include RefId=\"SelectColumns\" />";
            var doc = CreateXmlDocument(xml);
            var statement = CreateStatement();
            var builder = new IncludeBuilder();

            var result = builder.Build(doc.DocumentElement, statement);

            result.Should().BeOfType<Include>();
            var include = result as Include;
            include.RefId.Should().Be("TestScope.SelectColumns");
            include.Prepend.Should().BeNull();
            include.Required.Should().BeFalse();
        }

        [Fact]
        public void Should_PrefixScope_When_RefIdHasNoDot()
        {
            var xml = "<Include RefId=\"SelectColumns\" />";
            var doc = CreateXmlDocument(xml);
            var statement = CreateStatement("MyScope");
            var builder = new IncludeBuilder();

            var result = builder.Build(doc.DocumentElement, statement);

            var include = result as Include;
            include.RefId.Should().Be("MyScope.SelectColumns");
        }

        [Fact]
        public void Should_NotPrefixScope_When_RefIdHasDot()
        {
            var xml = "<Include RefId=\"OtherScope.SelectColumns\" />";
            var doc = CreateXmlDocument(xml);
            var statement = CreateStatement("MyScope");
            var builder = new IncludeBuilder();

            var result = builder.Build(doc.DocumentElement, statement);

            var include = result as Include;
            include.RefId.Should().Be("OtherScope.SelectColumns");
        }

        [Fact]
        public void Should_SetPrependOnInclude_When_PrependAttributeProvided()
        {
            var xml = "<Include RefId=\"Columns\" Prepend=\",\" />";
            var doc = CreateXmlDocument(xml);
            var statement = CreateStatement();
            var builder = new IncludeBuilder();

            var result = builder.Build(doc.DocumentElement, statement);

            var include = result as Include;
            include.Prepend.Should().Be(",");
        }

        [Fact]
        public void Should_SetRequiredOnInclude_When_RequiredAttributeProvided()
        {
            var xml = "<Include RefId=\"Columns\" Required=\"true\" />";
            var doc = CreateXmlDocument(xml);
            var statement = CreateStatement();
            var builder = new IncludeBuilder();

            var result = builder.Build(doc.DocumentElement, statement);

            var include = result as Include;
            include.Required.Should().BeTrue();
        }

        #endregion

        #region ForBuilder

        [Fact]
        public void Should_BuildForTag_When_ForBuilderCalled()
        {
            var xml = "<For Property=\"Ids\" Open=\"(\" Close=\")\" Separator=\",\" />";
            var doc = CreateXmlDocument(xml);
            var statement = CreateStatement();
            var builder = new ForBuilder();

            var result = builder.Build(doc.DocumentElement, statement);

            result.Should().BeOfType<For>();
            var forTag = result as For;
            forTag.Property.Should().Be("Ids");
            forTag.Open.Should().Be("(");
            forTag.Close.Should().Be(")");
            forTag.Separator.Should().Be(",");
            forTag.Key.Should().Be("Ids");
            forTag.Statement.Should().BeSameAs(statement);
        }

        [Fact]
        public void Should_DefaultKeyToProperty_When_KeyAttributeNotProvided()
        {
            var xml = "<For Property=\"UserIds\" />";
            var doc = CreateXmlDocument(xml);
            var statement = CreateStatement();
            var builder = new ForBuilder();

            var result = builder.Build(doc.DocumentElement, statement);

            var forTag = result as For;
            forTag.Key.Should().Be("UserIds");
        }

        [Fact]
        public void Should_SetCustomKey_When_KeyAttributeProvided()
        {
            var xml = "<For Property=\"Ids\" Key=\"ItemId\" />";
            var doc = CreateXmlDocument(xml);
            var statement = CreateStatement();
            var builder = new ForBuilder();

            var result = builder.Build(doc.DocumentElement, statement);

            var forTag = result as For;
            forTag.Key.Should().Be("ItemId");
        }

        [Fact]
        public void Should_SetPrependOnForTag_When_PrependAttributeProvided()
        {
            var xml = "<For Property=\"Ids\" Prepend=\"AND\" />";
            var doc = CreateXmlDocument(xml);
            var statement = CreateStatement();
            var builder = new ForBuilder();

            var result = builder.Build(doc.DocumentElement, statement);

            var forTag = result as For;
            forTag.Prepend.Should().Be("AND");
        }

        [Fact]
        public void Should_SetRequiredOnForTag_When_RequiredAttributeProvided()
        {
            var xml = "<For Property=\"Ids\" Required=\"true\" />";
            var doc = CreateXmlDocument(xml);
            var statement = CreateStatement();
            var builder = new ForBuilder();

            var result = builder.Build(doc.DocumentElement, statement);

            var forTag = result as For;
            forTag.Required.Should().BeTrue();
        }

        [Fact]
        public void Should_CreateEmptyChildTags_When_ForBuilderBuildsTag()
        {
            var xml = "<For Property=\"Ids\" />";
            var doc = CreateXmlDocument(xml);
            var statement = CreateStatement();
            var builder = new ForBuilder();

            var result = builder.Build(doc.DocumentElement, statement);

            var forTag = result as For;
            forTag.ChildTags.Should().NotBeNull();
            forTag.ChildTags.Should().BeEmpty();
        }

        #endregion
    }
}
