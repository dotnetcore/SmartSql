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

    }

}
