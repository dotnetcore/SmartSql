using System;
using System.Collections.Generic;
using System.Xml;
using FluentAssertions;
using Moq;
using SmartSql.Configuration;
using SmartSql.Configuration.Tags;
using SmartSql.Configuration.Tags.TagBuilders;
using SmartSql.DataSource;
using SmartSql.Exceptions;
using SmartSql.IdGenerator;
using Xunit;
using IdGenTag = SmartSql.Configuration.Tags.IdGenerator;
using TagIdGenBuilder = SmartSql.Configuration.Tags.TagBuilders.IdGeneratorBuilder;

namespace SmartSql.Test.Unit.Tags
{
    public class IdGeneratorBuilderTests
    {
        private static XmlDocument CreateXmlDocument(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc;
        }

        private static Statement CreateStatement(IIdGenerator defaultIdGen = null)
        {
            var idGens = new Dictionary<string, IIdGenerator>();
            if (defaultIdGen != null)
            {
                idGens["Default"] = defaultIdGen;
            }

            return new Statement
            {
                SqlMap = new SqlMap
                {
                    SmartSqlConfig = new SmartSqlConfig
                    {
                        Database = new Database
                        {
                            DbProvider = new DbProvider { ParameterPrefix = "@" }
                        },
                        IdGenerators = idGens
                    }
                }
            };
        }

        [Fact]
        public void Should_BuildIdGeneratorTag_When_XmlIsValid()
        {
            var mockIdGen = new Mock<IIdGenerator>();
            var statement = CreateStatement(mockIdGen.Object);
            var xml = "<IdGenerator Id=\"UserId\" />";
            var doc = CreateXmlDocument(xml);
            var builder = new TagIdGenBuilder();

            var result = builder.Build(doc.DocumentElement, statement);

            result.Should().BeOfType<IdGenTag>();
            var tag = result as IdGenTag;
            tag.Id.Should().Be("UserId");
            tag.IdGen.Should().BeSameAs(mockIdGen.Object);
        }

        [Fact]
        public void Should_UseFirstIdGenerator_When_NameNotSpecified()
        {
            var mockIdGen = new Mock<IIdGenerator>();
            var statement = CreateStatement(mockIdGen.Object);
            var xml = "<IdGenerator Id=\"OrderId\" />";
            var doc = CreateXmlDocument(xml);
            var builder = new TagIdGenBuilder();

            var result = builder.Build(doc.DocumentElement, statement);

            var tag = result as IdGenTag;
            tag.IdGen.Should().BeSameAs(mockIdGen.Object);
        }

        [Fact]
        public void Should_UseNamedIdGenerator_When_NameSpecified()
        {
            var defaultIdGen = new Mock<IIdGenerator>().Object;
            var namedIdGen = new Mock<IIdGenerator>().Object;
            var idGens = new Dictionary<string, IIdGenerator>
            {
                { "Default", defaultIdGen },
                { "Snowflake", namedIdGen }
            };

            var statement = new Statement
            {
                SqlMap = new SqlMap
                {
                    SmartSqlConfig = new SmartSqlConfig
                    {
                        Database = new Database
                        {
                            DbProvider = new DbProvider { ParameterPrefix = "@" }
                        },
                        IdGenerators = idGens
                    }
                }
            };

            var xml = "<IdGenerator Id=\"OrderId\" Name=\"Snowflake\" />";
            var doc = CreateXmlDocument(xml);
            var builder = new TagIdGenBuilder();

            var result = builder.Build(doc.DocumentElement, statement);

            var tag = result as IdGenTag;
            tag.IdGen.Should().BeSameAs(namedIdGen);
        }

        [Fact]
        public void Should_ThrowException_When_NamedIdGeneratorNotFound()
        {
            var mockIdGen = new Mock<IIdGenerator>();
            var statement = CreateStatement(mockIdGen.Object);
            var xml = "<IdGenerator Id=\"OrderId\" Name=\"NonExistent\" />";
            var doc = CreateXmlDocument(xml);
            var builder = new TagIdGenBuilder();

            var act = () => builder.Build(doc.DocumentElement, statement);

            act.Should().Throw<SmartSqlException>()
                .WithMessage("*NonExistent*");
        }

        [Fact]
        public void Should_SetAssignToTrue_When_AssignAttributeNotPresent()
        {
            var mockIdGen = new Mock<IIdGenerator>();
            var statement = CreateStatement(mockIdGen.Object);
            var xml = "<IdGenerator Id=\"UserId\" />";
            var doc = CreateXmlDocument(xml);
            var builder = new TagIdGenBuilder();

            var result = builder.Build(doc.DocumentElement, statement);

            var tag = result as IdGenTag;
            tag.Assign.Should().BeTrue();
        }

        [Fact]
        public void Should_SetAssignToFalse_When_AssignAttributeIsFalse()
        {
            var mockIdGen = new Mock<IIdGenerator>();
            var statement = CreateStatement(mockIdGen.Object);
            var xml = "<IdGenerator Id=\"UserId\" Assign=\"false\" />";
            var doc = CreateXmlDocument(xml);
            var builder = new TagIdGenBuilder();

            var result = builder.Build(doc.DocumentElement, statement);

            var tag = result as IdGenTag;
            tag.Assign.Should().BeFalse();
        }

        [Fact]
        public void Should_SetStatement_When_Built()
        {
            var mockIdGen = new Mock<IIdGenerator>();
            var statement = CreateStatement(mockIdGen.Object);
            var xml = "<IdGenerator Id=\"UserId\" />";
            var doc = CreateXmlDocument(xml);
            var builder = new TagIdGenBuilder();

            var result = builder.Build(doc.DocumentElement, statement);

            var tag = result as IdGenTag;
            tag.Statement.Should().BeSameAs(statement);
        }
    }
}
