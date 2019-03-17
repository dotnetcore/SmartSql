using System;
using Xunit;
using SmartSql.TypeHandlers;
using System.Reflection;
using System.Data;
using System.Dynamic;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SmartSql.Test.Entities;

namespace SmartSql.Test.Unit
{
    public class CommonTest
    {
        [Fact]
        public void Test1()
        {
            var yes = new AllPrimitive() as dynamic;
            var id = yes.Id;

            IList<dynamic> list = new List<dynamic>();
            IDictionary<string, object> exObj = new ExpandoObject();
            exObj.Add("1", 2);
            list.Add(exObj);
            var type = typeof(ITypeHandler<>);
            var typeStr = Assembly.Load("SmartSql").GetType("SmartSql.TypeHandlers.ITypeHandler`1");
        }
    }


}
