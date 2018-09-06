using SmartSql.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.UTests.Utils
{
    public class ObjectUtils_Tests
    {
        [Fact]
        public void ToDictionary_IgnorePropNameCase()
        {
            var obj = new { Name = "Ahoo" };
            var dic = ObjectUtils.ToDicDbParameters(obj, true);
            var name = dic["name"];
        }
        [Fact]
        public void ToDictionary()
        {
            var obj = new { Name = "Ahoo", name = "Ahoo", nAme = "Ahoo" };
            var dic = ObjectUtils.ToDicDbParameters(obj, false);
        }

        public void GetNestedVal()
        {
            var first = new First
            {
                Second = new Second
                {
                    Third = new Third
                    {
                        Fourth = new Fourth
                        {
                            Value = "GoodJob"
                        }
                    }
                }
            };

            var val = first.Second.Third.Fourth.Value;
        }
    }
    public class Second
    {
        public Third Third { get; set; }
    }
    public class Third
    {
        public Fourth Fourth { get; set; }
    }
    public class Fourth
    {
        public string Value { get; set; }
    }
    public class First
    {
        public Second Second { get; set; }
    }
}
