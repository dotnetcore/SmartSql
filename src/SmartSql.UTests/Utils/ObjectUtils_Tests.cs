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
        [Fact]
        public void ToDictionaryA()
        {
            var req = new TestRequest
            {
                TestId = 11111
            };
            dynamic type0 = new { Id = 1 }.GetType();
            var type = new { Id = 1 }.GetType();
            var type1 = new { Id = 1 }.GetType();
            var dic = ObjectUtils.ToDicDbParameters(new { FId = req.TestId, Name = "" }, false);
            var dic1 = ObjectUtils.ToDicDbParameters(new { Id = req.TestId, Name = "" }, false);
            var dic2 = ObjectUtils.ToDicDbParameters(new { Id = req.TestId, Name = "" }, false);
        }
    }

    public class TestRequest
    {
        public long TestId { get; set; }
    }

}
