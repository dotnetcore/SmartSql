using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using SmartSql.Common;
using SmartSql.SqlMap;

namespace SmartSql.Tests
{
    public class SmartSqlMapConfig_Test
    {
        [Fact]
        public void Watch()
        {
            SmartSqlMapper mapper = new SmartSqlMapper();

            mapper.Dispose();

        }
    }
}
