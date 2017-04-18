using SmartSql.SqlMap.Tags;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.Tests.SqlMap.Tag
{
    public class IsNotEmpty_Test
    {
        [Fact]
        public void ReturnSql()
        {
            IsNotEmpty isNotEmpty = new IsNotEmpty
            {
                //BodyText = "",
                Prepend = "And Id",
                Property = "Ids",
                In = true
            };
            string sql = isNotEmpty.BuildSql(new { Ids = new long[] { 1, 2 } },"@");
            Assert.NotNull(sql);
        }
    }
}
