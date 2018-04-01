using SmartSql.Abstractions;
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
                Property = "Ids"
            };
            string sql = isNotEmpty.BuildSql(new RequestContext
            {
                 
            });
            Assert.NotNull(sql);
        }
    }
}
