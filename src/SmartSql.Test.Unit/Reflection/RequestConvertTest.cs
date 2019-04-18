using SmartSql.Reflection;
using SmartSql.Reflection.Convert;
using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit.Reflection
{
    public class RequestConvertTest
    {
        [Fact]
        public void Convert()
        {
            var user = new User { Id = 0, UserName = "1" };
            var dic = RequestConvert.Instance.ToSqlParameters(user, true);
            var dic_1 = RequestConvert.Instance.ToSqlParameters(user, false);
            var dic_cache = RequestConvertCache<User, IgnoreCaseType>.Convert(user);
            var dic_cache_1 = RequestConvertCache<User>.Convert(user);
        }
        [Fact]
        public void Convert_Dy()
        {
            var dic = RequestConvert.Instance.ToSqlParameters(new { Id = 0, Name = 1 }, true);

        }
    }
}
