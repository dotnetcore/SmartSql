using System;
using System.Xml;
using FluentAssertions;
using Moq;
using SmartSql.Configuration;
using SmartSql.Configuration.Tags;
using SmartSql.Configuration.Tags.TagBuilders;
using SmartSql.DataSource;
using SmartSql.Exceptions;
using Xunit;

namespace SmartSql.Test.Unit.Tags;

public class TagBuildersTests
{
    private static XmlDocument CreateXmlDocument(string xml)
    {
        var doc = new XmlDocument();
        doc.LoadXml(xml);
        return doc;
    }

    private static Statement CreateStatement()
    {
        return new Statement
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
        };
    }

    [Fact]
    public void Should_GetXmlAttributeValue_When_AttributeExists()
    {
        var xml = "<Test Property=\"TestValue\" />";
        var doc = CreateXmlDocument(xml);
        var builder = new TestTagBuilder();

        var result = builder.GetXmlAttributeValue(doc.DocumentElement, "Property");

        result.Should().Be("TestValue");
    }

    [Fact]
    public void Should_ReturnNull_When_AttributeNotExists()
    {
        var xml = "<Test />";
        var doc = CreateXmlDocument(xml);
        var builder = new TestTagBuilder();

        var result = builder.GetXmlAttributeValue(doc.DocumentElement, "Missing");

        result.Should().BeNull();
    }

    [Fact]
    public void Should_GetXmlAttributeValueAsDecimal_When_ValidDecimal()
    {
        var xml = "<Test CompareValue=\"123.45\" />";
        var doc = CreateXmlDocument(xml);
        var builder = new TestTagBuilder();

        var result = builder.GetXmlAttributeValueAsDecimal(doc.DocumentElement, "CompareValue");

        result.Should().Be(123.45m);
    }

    [Fact]
    public void Should_Throw_When_InvalidDecimal()
    {
        var xml = "<Test CompareValue=\"not-a-number\" />";
        var doc = CreateXmlDocument(xml);
        var builder = new TestTagBuilder();

        var act = () => builder.GetXmlAttributeValueAsDecimal(doc.DocumentElement, "CompareValue");

        act.Should().Throw<SmartSqlException>();
    }

    [Fact]
    public void Should_GetPrepend_When_PrependAttributeExists()
    {
        var xml = "<Test Prepend=\"AND\" />";
        var doc = CreateXmlDocument(xml);
        var builder = new TestTagBuilder();

        var result = builder.GetPrepend(doc.DocumentElement);

        result.Should().Be("AND");
    }

    [Fact]
    public void Should_ReturnNull_When_PrependNotExists()
    {
        var xml = "<Test />";
        var doc = CreateXmlDocument(xml);
        var builder = new TestTagBuilder();

        var result = builder.GetPrepend(doc.DocumentElement);

        result.Should().BeNull();
    }

    [Fact]
    public void Should_GetProperty_When_PropertyAttributeExists()
    {
        var xml = "<Test Property=\"UserName\" />";
        var doc = CreateXmlDocument(xml);
        var builder = new TestTagBuilder();

        var result = builder.GetProperty(doc.DocumentElement);

        result.Should().Be("UserName");
    }

    [Fact]
    public void Should_Throw_When_PropertyNotExists()
    {
        var xml = "<Test />";
        var doc = CreateXmlDocument(xml);
        var builder = new TestTagBuilder();

        var act = () => builder.GetProperty(doc.DocumentElement);

        act.Should().Throw<SmartSqlException>();
    }

    [Fact]
    public void Should_GetRequired_When_RequiredAttributeExists()
    {
        var xml = "<Test Required=\"true\" />";
        var doc = CreateXmlDocument(xml);
        var builder = new TestTagBuilder();

        var result = builder.GetRequired(doc.DocumentElement);

        result.Should().BeTrue();
    }

    [Fact]
    public void Should_ReturnFalse_When_RequiredNotExists()
    {
        var xml = "<Test />";
        var doc = CreateXmlDocument(xml);
        var builder = new TestTagBuilder();

        var result = builder.GetRequired(doc.DocumentElement);

        result.Should().BeFalse();
    }

    [Fact]
    public void Should_GetMin_When_MinAttributeExists()
    {
        var xml = "<Test Min=\"5\" />";
        var doc = CreateXmlDocument(xml);
        var builder = new TestTagBuilder();

        var result = builder.GetMin(doc.DocumentElement);

        result.Should().Be(5);
    }

    [Fact]
    public void Should_ReturnNull_When_MinNotExists()
    {
        var xml = "<Test />";
        var doc = CreateXmlDocument(xml);
        var builder = new TestTagBuilder();

        var result = builder.GetMin(doc.DocumentElement);

        result.Should().BeNull();
    }

    [Fact]
    public void Should_GetCompareValue_When_CompareValueAttributeExists()
    {
        var xml = "<Test CompareValue=\"100\" />";
        var doc = CreateXmlDocument(xml);
        var builder = new TestTagBuilder();

        var result = builder.GetCompareValue(doc.DocumentElement);

        result.Should().Be("100");
    }

    [Fact]
    public void Should_GetCompareValueAsDecimal_When_ValidCompareValue()
    {
        var xml = "<Test CompareValue=\"999.99\" />";
        var doc = CreateXmlDocument(xml);
        var builder = new TestTagBuilder();

        var result = builder.GetCompareValueAsDecimal(doc.DocumentElement);

        result.Should().Be(999.99m);
    }

    [Fact]
    public void Should_TrimWhitespace_When_GettingAttributeValue()
    {
        var xml = "<Test Property=\"  TrimmedValue  \" />";
        var doc = CreateXmlDocument(xml);
        var builder = new TestTagBuilder();

        var result = builder.GetXmlAttributeValue(doc.DocumentElement, "Property");

        result.Should().Be("TrimmedValue");
    }

    [Fact]
    public void Should_HandleEmptyString_When_AttributeIsEmpty()
    {
        var xml = "<Test Property=\"\" />";
        var doc = CreateXmlDocument(xml);
        var builder = new TestTagBuilder();

        var result = builder.GetXmlAttributeValue(doc.DocumentElement, "Property");

        result.Should().BeEmpty();
    }

    [Fact]
    public void Should_ReturnFalse_When_RequiredIsFalse()
    {
        var xml = "<Test Required=\"false\" />";
        var doc = CreateXmlDocument(xml);
        var builder = new TestTagBuilder();

        var result = builder.GetRequired(doc.DocumentElement);

        result.Should().BeFalse();
    }

    [Fact]
    public void Should_ParseBoolean_When_RequiredIsTrueString()
    {
        var xml = "<Test Required=\"true\" />";
        var doc = CreateXmlDocument(xml);
        var builder = new TestTagBuilder();

        var result = builder.GetRequired(doc.DocumentElement);

        result.Should().BeTrue();
    }

    [Fact]
    public void Should_ParseBoolean_When_RequiredIsZero()
    {
        var xml = "<Test Required=\"0\" />";
        var doc = CreateXmlDocument(xml);
        var builder = new TestTagBuilder();

        var result = builder.GetRequired(doc.DocumentElement);

        result.Should().BeFalse();
    }

    [Fact]
    public void DynamicBuilder_Should_BuildDynamicTag()
    {
        var xml = "<Dynamic Prepend=\"WHERE\" Required=\"true\" Min=\"1\" />";
        var doc = CreateXmlDocument(xml);
        var statement = CreateStatement();
        var builder = new DynamicBuilder();

        var result = builder.Build(doc.DocumentElement, statement);

        result.Should().BeOfType<Dynamic>();
        var dynamic = result as Dynamic;
        dynamic.Prepend.Should().Be("WHERE");
        dynamic.Required.Should().BeTrue();
        dynamic.Min.Should().Be(1);
        dynamic.Statement.Should().BeSameAs(statement);
    }

    [Fact]
    public void DynamicBuilder_Should_CreateEmptyChildTags()
    {
        var xml = "<Dynamic />";
        var doc = CreateXmlDocument(xml);
        var statement = CreateStatement();
        var builder = new DynamicBuilder();

        var result = builder.Build(doc.DocumentElement, statement);

        var dynamic = result as Dynamic;
        dynamic.ChildTags.Should().NotBeNull();
        dynamic.ChildTags.Should().BeEmpty();
    }

    [Fact]
    public void Should_ThrowSmartSqlException_When_PropertyMissingAndRequired()
    {
        var xml = "<Test />";
        var doc = CreateXmlDocument(xml);
        var builder = new TestTagBuilder();

        var act = () => builder.GetProperty(doc.DocumentElement);

        act.Should().Throw<SmartSqlException>()
            .WithMessage("*Property*");
    }

    [Fact]
    public void Should_IncludeNodeInfoInException_When_PropertyMissing()
    {
        var xml = "<Test xmlns=\"http://test.com\" />";
        var doc = CreateXmlDocument(xml);
        var builder = new TestTagBuilder();

        var act = () => builder.GetProperty(doc.DocumentElement);

        act.Should().Throw<SmartSqlException>()
            .WithMessage("*Property*");
    }

    private class TestTagBuilder : AbstractTagBuilder
    {
        public override ITag Build(XmlNode xmlNode, Statement statement)
        {
            throw new NotImplementedException();
        }
    }
}
