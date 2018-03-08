using SmartSql.Abstractions;
using SmartSql.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Tests
{
    public abstract class TestBase : IDisposable
    {
        protected ISmartSqlMapper SqlMapper;
        public TestBase()
        {
            SqlMapper = new SmartSqlMapper();
        }


        public void Dispose()
        {
            SqlMapper.Dispose();
        }
    }

    public class T_Test
    {
        public long Id { get; set; }
        public String Name { get; set; }
        public int Status { get; set; }
    }
}
