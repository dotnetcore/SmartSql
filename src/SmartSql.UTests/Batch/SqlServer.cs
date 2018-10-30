using SmartSql.Abstractions;
using SmartSql.Batch;
using SmartSql.Batch.SqlServer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SmartSql.UTests.Batch
{
    public class SqlServer
    {
        readonly ISmartSqlMapper _sqlMapper;
        public SqlServer()
        {
            _sqlMapper = MapperContainer.Instance.GetSqlMapper("SmartSqlMapConfig.xml");
        }
        [Fact]
        public void Insert()
        {
            var batchInsertNum = 1000000;
            var batchTable = new DbTable("t_test");
            batchTable.AddColumn(new DbColumn
            {
                AutoIncrement = true,
                DataType = typeof(long),
                Name = "id"
            });
            batchTable.AddColumn("name", typeof(string));
            batchTable.AddColumn("no", typeof(int));
            for (var i = 0; i < batchInsertNum; i++)
            {
                var row = batchTable.AddRow();
                row["name"] = "SmartSql-" + Guid.NewGuid().ToString("N");
                row["no"] = i;
            }
            using (BatchInsert batchInsert = new BatchInsert(_sqlMapper))
            {
                batchInsert.Table = batchTable;
                batchInsert.AddColumnMapping(new ColumnMapping
                {
                    Column = "id",
                    Mapping = "Id"
                });
                batchInsert.AddColumnMapping(new ColumnMapping
                {
                    Column = "name",
                    Mapping = "Name"
                });
                batchInsert.AddColumnMapping(new ColumnMapping
                {
                    Column = "no",
                    Mapping = "No"
                });
                batchInsert.Insert();
            }
        }
        [Fact]
        public async Task InsertAsync()
        {
            var batchInsertNum = 1000000;
            var batchTable = new DbTable("t_test");
            batchTable.AddColumn(new DbColumn
            {
                AutoIncrement = true,
                DataType = typeof(long),
                Name = "id"
            });
            batchTable.AddColumn("name", typeof(string));
            batchTable.AddColumn("no", typeof(int));
            for (var i = 0; i < batchInsertNum; i++)
            {
                var row = batchTable.AddRow();
                row["name"] = "SmartSql-" + Guid.NewGuid().ToString("N");
                row["no"] = i;
            }
            using (BatchInsert batchInsert = new BatchInsert(_sqlMapper))
            {
                batchInsert.Table = batchTable;
                batchInsert.AddColumnMapping(new ColumnMapping
                {
                    Column = "id",
                    Mapping = "Id"
                });
                batchInsert.AddColumnMapping(new ColumnMapping
                {
                    Column = "name",
                    Mapping = "Name"
                });
                batchInsert.AddColumnMapping(new ColumnMapping
                {
                    Column = "no",
                    Mapping = "No"
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
