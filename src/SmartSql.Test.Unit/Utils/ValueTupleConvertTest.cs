using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Reflection;
using Xunit;

namespace SmartSql.Test.Unit.Utils
{
    public class ValueTupleConvertTest
    {
        [Fact]
        public void Convert()
        {
            var valTuple = ValueTupleConvert.Convert(typeof(ValueTuple<int, string, long>), new object[] { 1, "SmartSql", 2L });
            var typedVal = (ValueTuple<int, string, long>) valTuple;
            Assert.NotNull(valTuple);
        }
    }
}
