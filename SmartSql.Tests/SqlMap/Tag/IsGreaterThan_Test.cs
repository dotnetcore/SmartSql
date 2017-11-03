using SmartSql.SqlMap.Tags;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.Tests.SqlMap.Tag
{
    public class IsGreaterThan_Test
    {
        [Fact]
        public void ReturnSql()
        {
            IsGreaterThan tag = new IsGreaterThan
            {
                //BodyText = "",
                Prepend = "And Id",
                Property = "Status",
                CompareValue = "0",

            };
            string sql = tag.BuildSql(new RequestContext
            {
                Request = new { Status = OrderStatus.Done }
            }, "@");
            Assert.NotNull(sql);
        }

        public enum OrderStatus
        {
            /// <summary>
            /// 待解决
            /// </summary>
            Pending = 3,
            /// <summary>
            /// 已解决
            /// </summary>
            Done = 1,
            /// <summary>
            /// 已拒绝
            /// </summary>
            Refused = 2
        }
    }
}
