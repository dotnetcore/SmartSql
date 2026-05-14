using System;
using System.Collections.Generic;
using FluentAssertions;
using SmartSql.Data;
using Xunit;

namespace SmartSql.Test.Unit.Data
{
    public class SqlParameterCollectionTests
    {
        [Fact]
        public void Should_AddParameter_When_KeyDoesNotExist()
        {
            var collection = new SqlParameterCollection();
            var param = new SqlParameter("Name", "Alice");

            collection.Add(param);

            collection.Count.Should().Be(1);
            collection["Name"].Should().BeSameAs(param);
        }

        [Fact]
        public void Should_ReturnFalse_When_AddingDuplicateKey()
        {
            var collection = new SqlParameterCollection();
            collection.Add(new SqlParameter("Id", 1));

            var result = collection.TryAdd(new SqlParameter("Id", 2));

            result.Should().BeFalse();
            collection["Id"].Value.Should().Be(1);
        }

        [Fact]
        public void Should_GetParameter_When_KeyExists()
        {
            var collection = new SqlParameterCollection();
            collection.Add(new SqlParameter("Name", "Bob"));

            var found = collection.TryGetValue("Name", out var param);

            found.Should().BeTrue();
            param.Value.Should().Be("Bob");
        }

        [Fact]
        public void Should_ReturnFalse_When_KeyNotFound()
        {
            var collection = new SqlParameterCollection();

            var found = collection.TryGetValue("Missing", out var param);

            found.Should().BeFalse();
            param.Should().BeNull();
        }

        [Fact]
        public void Should_RemoveParameter_When_KeyExists()
        {
            var collection = new SqlParameterCollection();
            collection.Add(new SqlParameter("Id", 42));

            var removed = collection.Remove("Id");

            removed.Should().BeTrue();
            collection.Count.Should().Be(0);
            collection.ContainsKey("Id").Should().BeFalse();
        }

        [Fact]
        public void Should_ClearAll_When_ClearCalled()
        {
            var collection = new SqlParameterCollection();
            collection.Add(new SqlParameter("A", 1));
            collection.Add(new SqlParameter("B", 2));

            collection.Clear();

            collection.Count.Should().Be(0);
            collection.DbParameters.Count.Should().Be(0);
        }

        [Fact]
        public void Should_ResolveNestedProperty_When_DotNotationUsed()
        {
            var collection = new SqlParameterCollection();
            var user = new { Name = "Alice" };
            collection.Add(new SqlParameter("User", user));

            var found = collection.TryGetValue("User.Name", out var param);

            found.Should().BeTrue();
            param.Value.Should().Be("Alice");
        }

        [Fact]
        public void Should_ResolveIndexer_When_BracketNotationUsedWithArray()
        {
            var collection = new SqlParameterCollection();
            var ids = new[] { 10, 20, 30 };
            collection.Add(new SqlParameter("Ids", ids));

            var found = collection.TryGetValue("Ids[1]", out var param);

            found.Should().BeTrue();
            param.Value.Should().Be(20);
        }

        [Fact]
        public void Should_ResolveIndexer_When_BracketNotationUsedWithDictionary()
        {
            var collection = new SqlParameterCollection();
            var dict = new Dictionary<string, object> { { "Key1", "Value1" } };
            collection.Add(new SqlParameter("Dict", dict));

            var found = collection.TryGetValue("Dict[Key1]", out var param);

            found.Should().BeTrue();
            param.Value.Should().Be("Value1");
        }

        [Fact]
        public void Should_HandleCaseInsensitive_When_IgnoreCaseEnabled()
        {
            var collection = new SqlParameterCollection(ignoreCase: true);
            collection.Add(new SqlParameter("Name", "Alice"));

            var found = collection.TryGetValue("name", out var param);

            found.Should().BeTrue();
            param.Value.Should().Be("Alice");
        }

        [Fact]
        public void Should_CreateParameterFromKvp_When_RequestIsDictionary()
        {
            var dict = new Dictionary<string, object> { { "Id", 42 }, { "Name", "Test" } };
            var sqlParams = SqlParameterCollection.Create<object>(dict, false);

            sqlParams.TryGetValue("Id", out var idParam).Should().BeTrue();
            idParam.Value.Should().Be(42);
            sqlParams.TryGetValue("Name", out var nameParam).Should().BeTrue();
            nameParam.Value.Should().Be("Test");
        }

        [Fact]
        public void Should_ContainKey_When_ParameterExists()
        {
            var collection = new SqlParameterCollection();
            collection.Add(new SqlParameter("Id", 1));

            collection.ContainsKey("Id").Should().BeTrue();
            collection.ContainsKey("Missing").Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnEmptyCollection_When_RequestIsNull()
        {
            var sqlParams = SqlParameterCollection.Create<object>(null, false);

            sqlParams.Count.Should().Be(0);
        }

        [Fact]
        public void Should_ReturnSameCollection_When_RequestIsAlreadySqlParameterCollection()
        {
            var original = new SqlParameterCollection();
            original.Add(new SqlParameter("Id", 1));

            var result = SqlParameterCollection.Create<ISqlParameterCollection>(original, false);

            result.Should().BeSameAs(original);
        }

        [Fact]
        public void Should_TryGetParameterValue_When_GenericRequested()
        {
            var collection = new SqlParameterCollection();
            collection.Add(new SqlParameter("Count", 42));

            var found = collection.TryGetParameterValue<int>("Count", out var value);

            found.Should().BeTrue();
            value.Should().Be(42);
        }

        [Fact]
        public void Should_ReturnKeysAndValues_When_AccessingProperties()
        {
            var collection = new SqlParameterCollection();
            collection.Add(new SqlParameter("A", 1));
            collection.Add(new SqlParameter("B", 2));

            collection.Keys.Should().Contain("A", "B");
            collection.Values.Should().HaveCount(2);
        }

        [Fact]
        public void Should_ReplaceValue_When_IndexerSet()
        {
            var collection = new SqlParameterCollection();
            collection.Add(new SqlParameter("Id", 1));

            var newParam = new SqlParameter("Id", 99);
            collection["Id"] = newParam;

            collection["Id"].Value.Should().Be(99);
        }

        [Fact]
        public void Should_RegisterDbParameter_When_SourceParameterIsSet()
        {
            var collection = new SqlParameterCollection();
            var param = new SqlParameter("Id", 42);
            collection.Add(param);

            var dbParam = new Microsoft.Data.Sqlite.SqliteParameter("Id", 42);
            param.SourceParameter = dbParam;

            collection.DbParameters.Should().ContainKey("Id");
        }

        [Fact]
        public void Should_TryAddByPropertyName_When_ValueIsObject()
        {
            var collection = new SqlParameterCollection();

            var result = collection.TryAdd("Name", "Alice");

            result.Should().BeTrue();
            collection.TryGetValue("Name", out var param).Should().BeTrue();
            param.Value.Should().Be("Alice");
        }

        [Fact]
        public void Should_NotBeReadOnly_When_Created()
        {
            var collection = new SqlParameterCollection();

            collection.IsReadOnly.Should().BeFalse();
        }

        [Fact]
        public void Should_EnumerateAllParameters_When_GetEnumeratorCalled()
        {
            var collection = new SqlParameterCollection();
            collection.Add(new SqlParameter("A", 1));
            collection.Add(new SqlParameter("B", 2));

            var items = new List<KeyValuePair<string, SqlParameter>>();
            foreach (var item in collection)
            {
                items.Add(item);
            }

            items.Should().HaveCount(2);
            items.Should().Contain(p => p.Key == "A" && (int)p.Value.Value == 1);
            items.Should().Contain(p => p.Key == "B" && (int)p.Value.Value == 2);
        }

        [Fact]
        public void Should_RemoveKeyValuePair_When_KeyMatches()
        {
            var collection = new SqlParameterCollection();
            var param = new SqlParameter("Id", 1);
            collection.Add(param);

            var removed = collection.Remove(new KeyValuePair<string, SqlParameter>("Id", param));

            removed.Should().BeTrue();
            collection.Count.Should().Be(0);
        }

        [Fact]
        public void Should_AddKeyValuePair_When_UsedAsDictionary()
        {
            var collection = new SqlParameterCollection();
            var param = new SqlParameter("Id", 1);

            collection.Add(new KeyValuePair<string, SqlParameter>("Id", param));

            collection.Count.Should().Be(1);
            collection["Id"].Should().BeSameAs(param);
        }

        [Fact]
        public void Should_ReturnFalseDefault_When_TryGetParameterValueOnMissingKey()
        {
            var collection = new SqlParameterCollection();

            var found = collection.TryGetParameterValue<int>("Missing", out var value);

            found.Should().BeFalse();
            value.Should().Be(0);
        }
    }
}
