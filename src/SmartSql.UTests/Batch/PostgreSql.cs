using Npgsql;
using SmartSql.Abstractions;
using SmartSql.Batch;
using SmartSql.Batch.PostgreSql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SmartSql.UTests.Batch
{
    public class PostgreSql : IDisposable
    {
        readonly ISmartSqlMapper _sqlMapper;
        public PostgreSql()
        {
            _sqlMapper = MapperContainer.Instance.GetSqlMapper("SmartSqlMapConfig-PostgreSql.xml");
        }
        [Fact]
        public void Insert()
        {
            var batchInsertNum = 1000000;
            var batchTable = new DbTable("t_test");
            batchTable.AddColumn("name");
            batchTable.AddColumn("no");
            for (var i = 0; i < batchInsertNum; i++)
            {
                var row = batchTable.AddRow();
                row["name"] = "SmartSql-" + Guid.NewGuid().ToString("N");
                row["no"] = i;
            }
            using (BatchInsert batchInsert = new BatchInsert(_sqlMapper))
            {
                batchInsert.Table = batchTable;
                batchInsert.Insert();
            }
        }
        [Fact]
        public async Task InsertAsync()
        {
            var batchInsertNum = 1000000;
            var batchTable = new DbTable("t_test");
            batchTable.AddColumn("name");
            batchTable.AddColumn("no");
            for (var i = 0; i < batchInsertNum; i++)
            {
                var row = batchTable.AddRow();
                row["name"] = "SmartSql-" + Guid.NewGuid().ToString("N");
                row["no"] = i;
            }
            using (BatchInsert batchInsert = new BatchInsert(_sqlMapper))
            {
                batchInsert.Table = batchTable;
                await batchInsert.InsertAsync();
            }
        }
        public void Dispose()
        {
            MapperContainer.Instance.Dispose();
        }
    }
}
