using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using FluentAssertions;
using SmartSql.Data;
using Xunit;

namespace SmartSql.Test.Unit.Data
{
    public class DynamicRowMetaObjectTests
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
        public void Should_GetMemberValue_When_DynamicAccessByMember()
        {
            dynamic row = CreateRow();

            var id = row.Id;

            ((object)id).Should().Be(1L);
        }

        [Fact]
        public void Should_GetMemberValue_When_DynamicAccessName()
        {
            dynamic row = CreateRow();

            var name = row.Name;

            ((object)name).Should().Be("Test");
        }

        [Fact]
        public void Should_InvokeMemberAsGet_When_DynamicInvokeUsed()
        {
            dynamic row = CreateRow();

            // DynamicRowMetaObject.BindInvokeMember delegates to GetValue via the indexer,
            // so invoking a member name returns the dictionary value for that key.
            var id = row.Id();

            ((object)id).Should().Be(1L);
        }

        [Fact]
        public void Should_GetNull_When_DynamicAccessMissingMember()
        {
            dynamic row = CreateRow();

            var missing = row.NonExistent;

            ((object)missing).Should().BeNull();
        }

        [Fact]
        public void Should_HandleMultipleColumns_When_DynamicAccess()
        {
            var columns = new Dictionary<string, int>
            {
                { "FirstName", 0 },
                { "LastName", 1 },
                { "Age", 2 }
            };
            var values = new object[] { "John", "Doe", 30 };
            dynamic row = new DynamicRow(columns, values);

            ((object)row.FirstName).Should().Be("John");
            ((object)row.LastName).Should().Be("Doe");
            ((object)row.Age).Should().Be(30);
        }

        [Fact]
        public void Should_ReturnMetaObject_When_GetMetaObjectCalled()
        {
            var row = CreateRow();

            var metaObject = row.GetMetaObject(Expression.Constant(row));

            metaObject.Should().NotBeNull();
            metaObject.Should().BeOfType<DynamicRowMetaObject>();
        }

        [Fact]
        public void Should_SetMemberValue_When_UsingDictionaryIndexer()
        {
            // DynamicRowMetaObject.BindSetMember uses SetValue which is not defined on DynamicRow.
            // Setting values works via the IDictionary<string,object> indexer instead.
            var row = CreateRow();
            row["Name"] = "Updated";

            row["Name"].Should().Be("Updated");
        }

        [Fact]
        public void Should_ReadAllColumns_When_UsingDynamicAccess()
        {
            var columns = new Dictionary<string, int>
            {
                { "A", 0 },
                { "B", 1 },
                { "C", 2 },
                { "D", 3 }
            };
            var values = new object[] { 1, "two", 3.0, true };
            dynamic row = new DynamicRow(columns, values);

            ((object)row.A).Should().Be(1);
            ((object)row.B).Should().Be("two");
            ((object)row.C).Should().Be(3.0);
            ((object)row.D).Should().Be(true);
        }

        [Fact]
        public void Should_ReturnNull_When_GettingMissingDynamicMember()
        {
            dynamic row = CreateRow();

            var val = row.DoesNotExist;

            ((object)val).Should().BeNull();
        }

        [Fact]
        public void Should_InvokeMissingMemberAsNull_When_DynamicInvokeUsed()
        {
            dynamic row = CreateRow();

            var val = row.DoesNotExist();

            ((object)val).Should().BeNull();
        }
    }
}
