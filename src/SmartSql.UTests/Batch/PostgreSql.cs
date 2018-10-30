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
            batchTable.AddColumn("Name");
            batchTable.AddColumn("No");
            for (var i = 0; i < batchInsertNum; i++)
            {
                var row = batchTable.AddRow();
                row["Name"] = "SmartSql-" + Guid.NewGuid().ToString("N");
                row["No"] = i;
            }
            using (BatchInsert batchInsert = new BatchInsert(_sqlMapper))
            {
                batchInsert.Table = batchTable;
                batchInsert.AddColumnMapping(new ColumnMapping
                {
                    Column = "Name",
                    Mapping = "Name"
                });
                batchInsert.AddColumnMapping(new ColumnMapping
                {
                    Column = "No",
                    Mapping = "no"
                });
                batchInsert.Insert();
            }
        }
        [Fact]
        public async Task InsertAsync()
        {
            var batchInsertNum = 1000000;
            var batchTable = new DbTable("t_test");
            batchTable.AddColumn("Name");
            batchTable.AddColumn("No");
            for (var i = 0; i < batchInsertNum; i++)
            {
                var row = batchTable.AddRow();
                row["Name"] = "SmartSql-" + Guid.NewGuid().ToString("N");
                row["No"] = i;
            }
            using (BatchInsert batchInsert = new BatchInsert(_sqlMapper))
            {
                batchInsert.Table = batchTable;
                batchInsert.AddColumnMapping(new ColumnMapping
                {
                    Column = "Name",
                    Mapping = "name"
                });
                batchInsert.AddColumnMapping(new ColumnMapping
                {
                    Column = "No",
                    Mapping = "no"
                });
                await batchInsert.InsertAsync();
            }
        }
        public void Dispose()
        {
            MapperContainer.Instance.Dispose();
        }
    }
}
