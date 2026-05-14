using System;
using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using SmartSql.Reflection;
using Xunit;

namespace SmartSql.Test.Unit.Reflection;

public class PropertyChainHolderTests
{
    private class Address
    {
        public string City { get; set; }
        internal string InternalCode { get; set; }
    }

    private class Order
    {
        public Address ShippingAddress { get; set; }
    }

    private class ReadOnlyOrder
    {
        public Address BillingAddress { get; } = new Address();
    }

    [Fact]
    public void Should_SetLastPropertyAsProperty_When_ChainProvided()
    {
        var addressProp = typeof(Order).GetProperty("ShippingAddress");
        var cityProp = typeof(Address).GetProperty("City");
        var chain = new List<PropertyInfo> { addressProp, cityProp };

        var holder = new PropertyChainHolder(chain, null);

        holder.Property.Should().BeSameAs(cityProp);
    }

    [Fact]
    public void Should_SetPropertyChain_When_ChainProvided()
    {
        var addressProp = typeof(Order).GetProperty("ShippingAddress");
        var cityProp = typeof(Address).GetProperty("City");
        var chain = new List<PropertyInfo> { addressProp, cityProp };

        var holder = new PropertyChainHolder(chain, null);

        holder.PropertyChain.Should().HaveCount(2);
        holder.PropertyChain[0].Should().BeSameAs(addressProp);
        holder.PropertyChain[1].Should().BeSameAs(cityProp);
    }

    [Fact]
    public void Should_SetTypeHandler_When_Provided()
    {
        var cityProp = typeof(Address).GetProperty("City");
        var chain = new List<PropertyInfo> { cityProp };

        var holder = new PropertyChainHolder(chain, "JsonTypeHandler");

        holder.TypeHandler.Should().Be("JsonTypeHandler");
    }

    [Fact]
    public void Should_BeChain_When_Accessed()
    {
        var cityProp = typeof(Address).GetProperty("City");
        var chain = new List<PropertyInfo> { cityProp };

        var holder = new PropertyChainHolder(chain, null);

        holder.IsChain.Should().BeTrue();
    }

    [Fact]
    public void Should_ReturnPropertyType_When_Accessed()
    {
        var cityProp = typeof(Address).GetProperty("City");
        var chain = new List<PropertyInfo> { cityProp };

        var holder = new PropertyChainHolder(chain, null);

        holder.PropertyType.Should().Be(typeof(string));
    }

    [Fact]
    public void Should_BeWritable_When_AllPropertiesInChainAreWritable()
    {
        var addressProp = typeof(Order).GetProperty("ShippingAddress");
        var cityProp = typeof(Address).GetProperty("City");
        var chain = new List<PropertyInfo> { addressProp, cityProp };

        var holder = new PropertyChainHolder(chain, null);

        holder.CanWrite.Should().BeTrue();
    }

    [Fact]
    public void Should_NotBeWritable_When_AnyPropertyInChainIsReadOnly()
    {
        var addressProp = typeof(ReadOnlyOrder).GetProperty("BillingAddress");
        var cityProp = typeof(Address).GetProperty("City");
        var chain = new List<PropertyInfo> { addressProp, cityProp };

        var holder = new PropertyChainHolder(chain, null);

        holder.CanWrite.Should().BeFalse();
    }

    [Fact]
    public void Should_ReturnSetMethod_When_Accessed()
    {
        var cityProp = typeof(Address).GetProperty("City");
        var chain = new List<PropertyInfo> { cityProp };

        var holder = new PropertyChainHolder(chain, null);

        holder.SetMethod.Should().NotBeNull();
    }

    [Fact]
    public void Should_AllowTypeHandlerUpdate_When_SetAfterConstruction()
    {
        var cityProp = typeof(Address).GetProperty("City");
        var chain = new List<PropertyInfo> { cityProp };

        var holder = new PropertyChainHolder(chain, null);

        holder.TypeHandler = "NewHandler";

        holder.TypeHandler.Should().Be("NewHandler");
    }
}
