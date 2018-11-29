using SmartSql.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
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
                TestId = 11111,
                UUID = Guid.NewGuid()
            };
            Trace.WriteLine("----- test---");
            
            var dic1 = ObjectUtils.ToDicDbParameters(new  { UUID = req.UUID, TestId = req.TestId }, true);
            var dic11 = ObjectUtils.ToDicDbParameters(new { UUID = req.UUID, TestId = req.TestId }, true);
            var dic2 = ObjectUtils.ToDicDbParameters(new { TestId = req.TestId, UUID = req.UUID }, true);
        }
    }

    public class TestRequest
    {
        public long? TestId { get; set; }
        public Guid UUID { get; set; }
    }

}
