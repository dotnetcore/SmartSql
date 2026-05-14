using System;
using System.Collections.Generic;
using FluentAssertions;
using SmartSql.Utils;
using Xunit;

namespace SmartSql.Test.Unit.Utils;

public class TypeScanOptionsTests
{
    [Fact]
    public void Should_SetAssemblyString_When_Assigned()
    {
        var options = new TypeScanOptions { AssemblyString = "SmartSql" };

        options.AssemblyString.Should().Be("SmartSql");
    }

    [Fact]
    public void Should_HaveDefaultFilter_When_Created()
    {
        var options = new TypeScanOptions();

        options.Filter.Should().NotBeNull();
        options.Filter(typeof(string)).Should().BeTrue();
        options.Filter(typeof(int)).Should().BeTrue();
    }

    [Fact]
    public void Should_SetCustomFilter_When_Assigned()
    {
        var options = new TypeScanOptions
        {
            Filter = t => t.IsInterface
        };

        options.Filter(typeof(IDisposable)).Should().BeTrue();
        options.Filter(typeof(string)).Should().BeFalse();
    }

    [Fact]
    public void Should_UseTypeFilter_When_UseTypeFilterCalled()
    {
        var options = new TypeScanOptions();
        options.UseTypeFilter<IDisposable>();

        options.Filter(typeof(System.IO.MemoryStream)).Should().BeTrue();
        options.Filter(typeof(string)).Should().BeFalse();
    }

    [Fact]
    public void Should_FilterByBaseType_When_UseTypeFilterCalled()
    {
        var options = new TypeScanOptions();
        options.UseTypeFilter<Exception>();

        options.Filter(typeof(InvalidOperationException)).Should().BeTrue();
        options.Filter(typeof(string)).Should().BeFalse();
    }
}
