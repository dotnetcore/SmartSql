using SmartSql.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit.ConfigBuilder
{
    public class PropertiesTest
    {
        [Fact]
        public void GetPropertyValueWithName()
        {
            Properties properties = new Properties();
            properties.Import(new Dictionary<string, string> {
                { "SmartSql","Great"}
            });
            var propVal = properties.GetPropertyValue("${SmartSql}-Great");
            Assert.Equal("Great-Great", propVal);
        }

        [Fact]
        public void GetPropertyValue()
        {
            Properties properties = new Properties();
            properties.Import(new Dictionary<string, string> {
                { "SmartSql","Great"}
            });
            var propVal = properties.GetPropertyValue("${SmartSql}");
            Assert.Equal("Great", propVal);
        }
        [Fact]
        public void GetColonPropertyValue()
        {
            Properties properties = new Properties();
            properties.Import(new Dictionary<string, string> {
                { "SmartSql:Great","Yes"}
            });
            var propVal = properties.GetPropertyValue("${SmartSql:Great}");
            Assert.Equal("Yes", propVal);
        }
        [Fact]
        public void GetBackQuotePropertyValue()
        {
            Properties properties = new Properties();
            properties.Import(new Dictionary<string, string> {
                { "SmartSql`Great","Yes"}
            });
            var propVal = properties.GetPropertyValue("${SmartSql`Great}");
            Assert.Equal("Yes", propVal);
        }
    }
}
