using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using SmartSql.Data;
using Xunit;

namespace SmartSql.Test.Unit.Data;

public class DynamicRowTests
{
    private static DynamicRow CreateRow(
        IDictionary<string, int> columns = null,
        object[] values = null)
    {
        columns = columns ?? new Dictionary<string, int>
        {
            { "Id", 0 },
            { "Name", 1 }
        };
        values = values ?? new object[] { 1L, "Test" };
        return new DynamicRow(columns, values);
    }

    [Fact]
    public void Should_ReturnValue_When_IndexerGet()
    {
        var row = CreateRow();

        row["Id"].Should().Be(1L);
        row["Name"].Should().Be("Test");
    }

    [Fact]
    public void Should_ReturnNull_When_IndexerGetWithMissingKey()
    {
        var row = CreateRow();

        row["Missing"].Should().BeNull();
    }

    [Fact]
    public void Should_UpdateValue_When_IndexerSet()
    {
        var row = CreateRow();

        row["Name"] = "Updated";

        row["Name"].Should().Be("Updated");
    }

    [Fact]
    public void Should_NotThrow_When_IndexerSetWithMissingKey()
    {
        var row = CreateRow();

        var act = () => row["Missing"] = "value";

        act.Should().NotThrow();
    }

    [Fact]
    public void Should_ReturnKeys_When_AccessingKeysProperty()
    {
        var row = CreateRow();

        row.Keys.Should().BeEquivalentTo("Id", "Name");
    }

    [Fact]
    public void Should_ReturnValues_When_AccessingValuesProperty()
    {
        var row = CreateRow();

        var values = row.Values;
        values.Should().Contain(1L);
        values.Should().Contain("Test");
        values.Count.Should().Be(2);
    }

    [Fact]
    public void Should_ReturnCount_When_AccessingCountProperty()
    {
        var row = CreateRow();

        row.Count.Should().Be(2);
    }

    [Fact]
    public void Should_BeReadOnly()
    {
        var row = CreateRow();

        row.IsReadOnly.Should().BeTrue();
    }

    [Fact]
    public void Should_ReturnTrue_When_ContainsKey()
    {
        var row = CreateRow();

        row.ContainsKey("Id").Should().BeTrue();
        row.ContainsKey("Name").Should().BeTrue();
    }

    [Fact]
    public void Should_ReturnFalse_When_ContainsKeyMissing()
    {
        var row = CreateRow();

        row.ContainsKey("Missing").Should().BeFalse();
    }

    [Fact]
    public void Should_ReturnTrue_When_TryGetValueExists()
    {
        var row = CreateRow();

        var result = row.TryGetValue("Id", out var value);

        result.Should().BeTrue();
        value.Should().Be(1L);
    }

    [Fact]
    public void Should_ReturnFalse_When_TryGetValueMissing()
    {
        var row = CreateRow();

        var result = row.TryGetValue("Missing", out var value);

        result.Should().BeFalse();
        value.Should().BeNull();
    }

    [Fact]
    public void Should_UpdateExistingKey_When_AddCalledWithExistingKey()
    {
        var row = CreateRow();

        row.Add("Name", "NewName");

        row["Name"].Should().Be("NewName");
    }

    [Fact]
    public void Should_ReturnFalse_When_ContainsKeyValuePairWithMissingKey()
    {
        var row = CreateRow();

        row.Contains(new KeyValuePair<string, object>("Missing", 1)).Should().BeFalse();
    }

    [Fact]
    public void Should_ReturnTrue_When_ContainsKeyValuePairWithMatchingValue()
    {
        var row = CreateRow();
        var value = row["Id"];

        row.Contains(new KeyValuePair<string, object>("Id", value)).Should().BeTrue();
    }

    [Fact]
    public void Should_ReturnFalse_When_ContainsKeyValuePairWithDifferentValue()
    {
        var row = CreateRow();

        row.Contains(new KeyValuePair<string, object>("Id", "different")).Should().BeFalse();
    }

    [Fact]
    public void Should_ClearAll_When_ClearCalled()
    {
        var row = CreateRow();

        row.Clear();

        row.Keys.Should().BeEmpty();
    }

    [Fact]
    public void Should_RemoveKey_When_RemoveCalled()
    {
        var row = CreateRow();

        var result = row.Remove("Id");

        result.Should().BeTrue();
        row.ContainsKey("Id").Should().BeFalse();
    }

    [Fact]
    public void Should_ReturnFalse_When_RemoveMissingKey()
    {
        var row = CreateRow();

        var result = row.Remove("Missing");

        result.Should().BeFalse();
    }

    [Fact]
    public void Should_RemoveByKey_When_RemoveKeyValuePairCalled()
    {
        var row = CreateRow();

        var result = row.Remove(new KeyValuePair<string, object>("Name", "Test"));

        result.Should().BeTrue();
        row.ContainsKey("Name").Should().BeFalse();
    }

    [Fact]
    public void Should_EnumerateAllKeyValuePairs_When_GetEnumeratorCalled()
    {
        var row = CreateRow();

        var pairs = row.ToList();

        pairs.Should().Contain(p => p.Key == "Id" && Equals(p.Value, 1L));
        pairs.Should().Contain(p => p.Key == "Name" && Equals(p.Value, "Test"));
        pairs.Count.Should().Be(2);
    }

    [Fact]
    public void Should_CopyToArray_When_CopyToCalled()
    {
        var row = CreateRow();
        var array = new KeyValuePair<string, object>[2];

        row.CopyTo(array, 0);

        array[0].Key.Should().Be("Id");
        array[1].Key.Should().Be("Name");
    }

    [Fact]
    public void Should_ReturnNonGenericEnumerator_When_IEnumerableGetEnumeratorCalled()
    {
        var row = CreateRow();
        var enumerable = (System.Collections.IEnumerable)row;

        var enumerator = enumerable.GetEnumerator();

        enumerator.Should().NotBeNull();
        enumerator.MoveNext().Should().BeTrue();
    }

    [Fact]
    public void Should_ThrowIndexOutOfRange_When_AddNewKey()
    {
        // Known bug: DynamicRow.Add uses _values[_columns.Count] instead of
        // _values[_columns.Count - 1] after Array.Resize, causing IndexOutOfRangeException.
        var row = CreateRow();

        var act = () => row.Add("Age", 25);

        act.Should().Throw<IndexOutOfRangeException>();
    }
}
