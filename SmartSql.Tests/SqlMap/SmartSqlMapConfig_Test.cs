using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using SmartSql.Abstractions;
using System.Threading;

namespace SmartSql.Tests.SqlMap
{
    public class SmartSqlMapConfig_Test : TestBase
    {

        [Fact]
        public void Query_OnChangeConfig()
        {
            int i = 0;
            for (i = 0; i < 10; i++)
            {
                var list = SqlMapper.Query<T_Test>(new RequestContext
                {
                    Scope = "T_Test",
                    SqlId = "GetList",
                    Request = new { Ids = new long[] { 1, 2, 3, 4 } }
                });
                Thread.Sleep(50);
            }
        }
    }
}
