using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Utils;
using SmartSql.Utils.PropertyAccessor.Impl;
using Xunit;

namespace SmartSql.UTests.Utils
{

    public class GetAccessorFactory_Tests
    {
        private GetAccessorFactory _getAccessorFactory = new GetAccessorFactory();

        [Fact]
        public void CreateGet()
        {
            var obj = new { Id = 1 };
            var getId = _getAccessorFactory.CreateGet(obj.GetType(), "Id", false);
            var id = getId(obj);
        }
        [Fact]
        public void CreateGet_Null_Property()
        {
            var obj = new { Id = 1 };
            var getName = _getAccessorFactory.CreateGet(obj.GetType(), "Name", false);
            var name = getName(obj);
        }

        [Fact]
        public void GetNestedParamVal_Test()
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

            var val = GetAccessorUtil.GetValue(first, "Second.Third.Fourth.Value", false);
            Assert.Equal("GoodJob", val);
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
}
