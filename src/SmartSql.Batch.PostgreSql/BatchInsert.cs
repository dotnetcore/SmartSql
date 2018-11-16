using System;
using Npgsql;
using SmartSql.Abstractions;
using System.Linq;
using System.Threading.Tasks;

namespace SmartSql.Batch.PostgreSql
{
    public class BatchInsert : AbstractBatchInsert
    {
        public BatchInsert(ISmartSqlMapper sqlMapper) : base(sqlMapper)
        {

        }

        public override void Insert()
        {
            var dataSource = DataSourceFilter.Elect(new RequestContext { DataSourceChoice = DataSourceChoice.Write });
            var dbSession = SessionStore.GetOrAddDbSession(dataSource);
            dbSession.OpenConnection();
            var conn = dbSession.Connection as NpgsqlConnection;
            InitColumnMappings();
            var colNames = String.Join(",", ColumnMappings.Keys);
            var copyFromCommand = $"COPY {Table.Name} ({colNames}) FROM STDIN (FORMAT BINARY)";
            using (var writer = conn.BeginBinaryImport(copyFromCommand))
            {
                foreach (var row in Table.Rows)
                {
                    writer.StartRow();
                    foreach (var mappingKey in ColumnMappings.Keys)
                    {
                        var colMapping = ColumnMappings[mappingKey];
                        var dbCellVal = row[colMapping.Column];
                        if (!String.IsNullOrEmpty(colMapping.DataTypeName))
                        {
                            if (colMapping.DataTypeName.ToUpper() == "JSONB")
                            {
                                writer.Write(dbCellVal, NpgsqlTypes.NpgsqlDbType.Jsonb);
                            }
                            else
                            {
                                writer.Write(dbCellVal, colMapping.DataTypeName);
                            }
                        }
                        else
                        {
                            writer.Write(dbCellVal);
                        }
                    }
                }
                writer.Complete();
            }
        }
        public override async Task InsertAsync()
        {
            var dataSource = DataSourceFilter.Elect(new RequestContext { DataSourceChoice = DataSourceChoice.Write });
            var dbSession = SessionStore.GetOrAddDbSession(dataSource);
            await dbSession.OpenConnectionAsync();
            var conn = dbSession.Connection as NpgsqlConnection;
            InitColumnMappings();
            var colNames = String.Join(",", ColumnMappings.Keys);
            var copyFromCommand = $"COPY {Table.Name} ({colNames}) FROM STDIN (FORMAT BINARY)";
            using (var writer = conn.BeginBinaryImport(copyFromCommand))
            {
                foreach (var row in Table.Rows)
                {
                    writer.StartRow();
                    foreach (var mappingKey in ColumnMappings.Keys)
                    {
                        var colMapping = ColumnMappings[mappingKey];
                        var dbCellVal = row[colMapping.Column];
                        if (!String.IsNullOrEmpty(colMapping.DataTypeName))
                        {
                            writer.Write(dbCellVal, colMapping.DataTypeName);
                        }
                        else
                        {
                            writer.Write(dbCellVal);
                        }
                    }
                }
                writer.Complete();
            }
        }
    }
}
