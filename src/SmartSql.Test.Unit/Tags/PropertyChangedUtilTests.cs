using System;
using FluentAssertions;
using Moq;
using SmartSql.Configuration;
using SmartSql.Configuration.Tags;
using Xunit;

namespace SmartSql.Test.Unit.Tags;

public class PropertyChangedUtilTests
{
    [Fact]
    public void Should_ReturnTrue_When_PropertyChangedIsIgnore()
    {
        var propertyChanged = new Mock<IPropertyChanged>();
        propertyChanged.Setup(p => p.PropertyChanged).Returns(PropertyChangedState.Ignore);
        propertyChanged.Setup(p => p.Property).Returns("Name");

        var context = new RequestContext<object>();

        var result = PropertyChangedUtil.IsCondition(propertyChanged.Object, context);

        result.Should().BeTrue();
    }

    [Fact]
    public void Should_ReturnTrue_When_PropertyChangedIsChangedAndVersionIsPositive()
    {
        var propertyChanged = new Mock<IPropertyChanged>();
        propertyChanged.Setup(p => p.PropertyChanged).Returns(PropertyChangedState.Changed);
        propertyChanged.Setup(p => p.Property).Returns("Name");

        var context = new TestableRequestContext { PropertyVersion = 3 };

        var result = PropertyChangedUtil.IsCondition(propertyChanged.Object, context);

        result.Should().BeTrue();
    }

    [Fact]
    public void Should_ReturnFalse_When_PropertyChangedIsChangedAndVersionIsZero()
    {
        var propertyChanged = new Mock<IPropertyChanged>();
        propertyChanged.Setup(p => p.PropertyChanged).Returns(PropertyChangedState.Changed);
        propertyChanged.Setup(p => p.Property).Returns("Name");

        var context = new TestableRequestContext { PropertyVersion = 0 };

        var result = PropertyChangedUtil.IsCondition(propertyChanged.Object, context);

        result.Should().BeFalse();
    }

    [Fact]
    public void Should_ReturnTrue_When_PropertyChangedIsUnchangedAndVersionIsZero()
    {
        var propertyChanged = new Mock<IPropertyChanged>();
        propertyChanged.Setup(p => p.PropertyChanged).Returns(PropertyChangedState.Unchanged);
        propertyChanged.Setup(p => p.Property).Returns("Name");

        var context = new TestableRequestContext { PropertyVersion = 0 };

        var result = PropertyChangedUtil.IsCondition(propertyChanged.Object, context);

        result.Should().BeTrue();
    }

    [Fact]
    public void Should_ReturnFalse_When_PropertyChangedIsUnchangedAndVersionIsPositive()
    {
        var propertyChanged = new Mock<IPropertyChanged>();
        propertyChanged.Setup(p => p.PropertyChanged).Returns(PropertyChangedState.Unchanged);
        propertyChanged.Setup(p => p.Property).Returns("Name");

        var context = new TestableRequestContext { PropertyVersion = 5 };

        var result = PropertyChangedUtil.IsCondition(propertyChanged.Object, context);

        result.Should().BeFalse();
    }

    [Fact]
    public void Should_ReturnTrue_When_PropertyChangedIsChangedAndVersionIsMinusOne()
    {
        var propertyChanged = new Mock<IPropertyChanged>();
        propertyChanged.Setup(p => p.PropertyChanged).Returns(PropertyChangedState.Changed);
        propertyChanged.Setup(p => p.Property).Returns("Name");

        var context = new TestableRequestContext { PropertyVersion = -1 };

        var result = PropertyChangedUtil.IsCondition(propertyChanged.Object, context);

        result.Should().BeTrue();
    }

    [Fact]
    public void Should_ReturnTrue_When_PropertyChangedIsUnchangedAndVersionIsMinusOne()
    {
        var propertyChanged = new Mock<IPropertyChanged>();
        propertyChanged.Setup(p => p.PropertyChanged).Returns(PropertyChangedState.Unchanged);
        propertyChanged.Setup(p => p.Property).Returns("Name");

        var context = new TestableRequestContext { PropertyVersion = -1 };

        var result = PropertyChangedUtil.IsCondition(propertyChanged.Object, context);

        result.Should().BeTrue();
    }

    [Fact]
    public void Should_ReturnIgnore_When_GetPropertyChangedWithNoAttributeAndNoTrackEnabled()
    {
        var doc = new System.Xml.XmlDocument();
        var node = doc.CreateElement("Tag");
        var statement = new Statement { EnablePropertyChangedTrack = false };

        var result = PropertyChangedUtil.GetPropertyChanged(node, statement);

        result.Should().Be(PropertyChangedState.Ignore);
    }

    [Fact]
    public void Should_ReturnChanged_When_GetPropertyChangedWithNoAttributeAndTrackEnabled()
    {
        var doc = new System.Xml.XmlDocument();
        var node = doc.CreateElement("Tag");
        var statement = new Statement { EnablePropertyChangedTrack = true };

        var result = PropertyChangedUtil.GetPropertyChanged(node, statement);

        result.Should().Be(PropertyChangedState.Changed);
    }

    [Fact]
    public void Should_ReturnChanged_When_GetPropertyChangedAttributeIsChanged()
    {
        var doc = new System.Xml.XmlDocument();
        var node = doc.CreateElement("Tag");
        var attr = doc.CreateAttribute("PropertyChanged");
        attr.Value = "Changed";
        node.Attributes.Append(attr);
        var statement = new Statement();

        var result = PropertyChangedUtil.GetPropertyChanged(node, statement);

        result.Should().Be(PropertyChangedState.Changed);
    }

    [Fact]
    public void Should_ReturnUnchanged_When_GetPropertyChangedAttributeIsUnchanged()
    {
        var doc = new System.Xml.XmlDocument();
        var node = doc.CreateElement("Tag");
        var attr = doc.CreateAttribute("PropertyChanged");
        attr.Value = "Unchanged";
        node.Attributes.Append(attr);
        var statement = new Statement();

        var result = PropertyChangedUtil.GetPropertyChanged(node, statement);

        result.Should().Be(PropertyChangedState.Unchanged);
    }

    [Fact]
    public void Should_ReturnIgnore_When_GetPropertyChangedAttributeIsIgnore()
    {
        var doc = new System.Xml.XmlDocument();
        var node = doc.CreateElement("Tag");
        var attr = doc.CreateAttribute("PropertyChanged");
        attr.Value = "Ignore";
        node.Attributes.Append(attr);
        var statement = new Statement();

        var result = PropertyChangedUtil.GetPropertyChanged(node, statement);

        result.Should().Be(PropertyChangedState.Ignore);
    }

    [Fact]
    public void Should_ThrowException_When_GetPropertyChangedAttributeIsInvalid()
    {
        var doc = new System.Xml.XmlDocument();
        var node = doc.CreateElement("Tag");
        var attr = doc.CreateAttribute("PropertyChanged");
        attr.Value = "InvalidValue";
        node.Attributes.Append(attr);
        var statement = new Statement();

        var act = () => PropertyChangedUtil.GetPropertyChanged(node, statement);

        act.Should().Throw<SmartSql.Exceptions.SmartSqlException>();
    }

    private class TestableRequestContext : RequestContext<object>
    {
        public int PropertyVersion { get; set; }

        public override int GetPropertyVersion(string propName)
        {
            return PropertyVersion;
        }
    }
}
