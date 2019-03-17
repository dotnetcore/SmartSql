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
            properties.Load(new Dictionary<string, string> {
                { "Goodjob","Yes"}
            });
            var propVal = properties.GetPropertyValue("${Goodjob}-yes");
            Assert.Equal("Yes-yes", propVal);
        }

        [Fact]
        public void GetPropertyValue()
        {
            Properties properties = new Properties();
            properties.Load(new Dictionary<string, string> {
                { "Goodjob","Yes"}
            });
            var propVal = properties.GetPropertyValue("goodjob-yes");
            Assert.Equal("goodjob-yes", propVal);
        }
    }
}
