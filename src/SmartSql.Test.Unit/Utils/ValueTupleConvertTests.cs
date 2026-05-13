using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using SmartSql.Reflection;
using Xunit;

namespace SmartSql.Test.Unit.Utils
{
    public class ValueTupleConvertTests
    {
        [Fact]
        public void Should_ConvertToObjectArray_When_ValueTupleTypeProvided()
        {
            var valTuple = ValueTupleConvert.Convert(typeof(ValueTuple<int, string, long>), new object[] { 1, "SmartSql", 2L });
            var typedVal = (ValueTuple<int, string, long>)valTuple;

            valTuple.Should().NotBeNull();
        }
    }
}
