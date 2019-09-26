using System;
using System.Data.SqlClient;
using SmartSql.DataSource;
using Xunit;

namespace SmartSql.Test.Unit
{
    public class CommonTest
    {
        [Fact]
        public void Test1()
        {
            Console.WriteLine("Test1");
        }
    }

    public class CacheAttribute : Attribute
    {
        public Type Type { get; set; }
        public string[] Strings { get; set; }
    }
}