using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Reflection.TypeConstants;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Unit.Reflection
{
    public class DefaultTypeTest
    {
        [Fact]
        public void DefaultInt()
        {
            var defaultVal = DefaultType.GetDefaultField(typeof(int)).GetValue(null);
            Assert.Equal(0, defaultVal);
        }
        [Fact]
        public void DefaultDecimal()
        {
            var defaultVal = DefaultType.GetDefaultField(typeof(decimal)).GetValue(null);
            Assert.Equal(0M, defaultVal);
        }
        [Fact]
        public void DefaultString()
        {
            var defaultVal = DefaultType.GetDefaultField(typeof(string)).GetValue(null);
            Assert.Null(defaultVal);
        }
        [Fact]
        public void DefaultEnum()
        {
            var defaultVal = DefaultType.GetDefaultField(typeof(UserStatus)).GetValue(null);
            Assert.Equal(default(UserStatus), defaultVal);
        }
    }
}
