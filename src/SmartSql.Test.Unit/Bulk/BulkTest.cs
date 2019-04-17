using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using SmartSql.Bulk;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Unit.Bulk
{
    public class BulkTest
    {
        [Fact]
        public void ToDataTable()
        {
            
            var list = new List<AllPrimitive>();
            list.Add(new AllPrimitive
            {
                Id = 0
            });
            list.Add(new AllPrimitive
            {
                Id = 1
            });

            var dataTable = list.ToDataTable();
            Assert.NotNull(dataTable);
        }
    }
}
