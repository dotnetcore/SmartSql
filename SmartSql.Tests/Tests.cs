using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Reflection;
using System.Diagnostics;
using System.Linq;

namespace SmartSql.Tests
{
    public class Tests
    {
        [Fact]
        public void Dy()
        {
            for (int i = 0; i < 100000; i++)
            {


                var good = new { Guid = Guid.NewGuid(), Id = 1, Name = "Ahoo", PageIndex = 1, PageSize = 20, HHa = "adf" };
                var type = good.GetType();


                var properties = type.GetProperties();//.ToList().OrderBy(p => p.Name);
                StringBuilder strBuilder = new StringBuilder();
                foreach (var property in properties)
                {
                    var val = property.GetValue(good);
                    strBuilder.AppendFormat("&{0}", val);
                    //strBuilder.AppendFormat("&{0}={1}", property.Name, val);
                }
                string queryStr = strBuilder.ToString().Trim('&');
                //Trace.WriteLine(queryStr);
                //byte[] bytes = Encoding.UTF8.GetBytes(queryStr);
                //var queryBase64 = Convert.ToBase64String(bytes);
                //Trace.WriteLine(queryBase64);
            }
        }
    }
}
