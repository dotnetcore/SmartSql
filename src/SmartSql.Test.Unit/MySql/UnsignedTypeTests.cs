using System;
using SmartSql.DataSource;
using Xunit;

namespace SmartSql.Test.Unit.MySql
{
    public class UnsignedTypeTests
    {
        private ISqlMapper _sqlMapper;

        public UnsignedTypeTests()
        {
            _sqlMapper = new SmartSqlBuilder()
                .UseDataSource(DbProvider.MYSQL, "Data Source=local-vm;database=smartsql_db;uid=root;pwd=mysql")
                .UseAlias("MySqlTest-" + Guid.NewGuid())
                .Build().SqlMapper;
        }

        // [Fact]
        public void QueryUInt()
        {
            var entity = _sqlMapper.QuerySingle<UnsignedTypeTable>(
                new RequestContext
                {
                    RealSql = "select uint16 as UInt16 ,uint32 as UInt32 ,uint64 as UInt64 from t_test"
                });
            Assert.NotNull(entity);
        }

        // [Fact]
        public void InsertUInt()
        {
            var affected = _sqlMapper.Execute(
                new RequestContext<UnsignedTypeTable>
                {
                    RealSql = "insert t_test (uint16, uint32, uint64) values (?UInt16, ?UInt32, ?UInt64);",
                    Request = new UnsignedTypeTable
                    {
                        UInt16 = 16,
                        UInt32 = 32,
                        UInt64 = 64
                    }
                });
            Assert.Equal(1, affected);
        }
    }


    public class UnsignedTypeTable
    {
        public UInt16 UInt16 { get; set; }
        public UInt32 UInt32 { get; set; }
        public UInt64 UInt64 { get; set; }
    }
}