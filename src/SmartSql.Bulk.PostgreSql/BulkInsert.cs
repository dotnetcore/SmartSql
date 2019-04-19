using Npgsql;
using SmartSql.DbSession;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Data;

namespace SmartSql.Bulk.PostgreSql
{
    public class BulkInsert : AbstractBulkInsert
    {
        public BulkInsert(IDbSession dbSession) : base(dbSession)
        {

        }

        public const string DATA_TYPE_NAME = "DataTypeName";
        public override void Insert()
        {
            DbSession.Open();
            InsertImpl();
        }

        private void InsertImpl()
        {
            var conn = DbSession.Connection as NpgsqlConnection;
            var dataColumns = Table.Columns.Cast<DataColumn>();
            var colNamesStr = String.Join(",", dataColumns.Select(col => col.ColumnName));

            var copyFromCommand = $"COPY {Table.TableName} ({colNamesStr}) FROM STDIN (FORMAT BINARY)";
            using (var writer = conn.BeginBinaryImport(copyFromCommand))
            {
                foreach (DataRow row in Table.Rows)
                {
                    writer.StartRow();
                    foreach (var dataColumn in dataColumns)
                    {
                        var dbCellVal = row[dataColumn];
                        if (dataColumn.ExtendedProperties.ContainsKey(DATA_TYPE_NAME))
                        {
                            var dataTypeName = dataColumn.ExtendedProperties[DATA_TYPE_NAME].ToString();
                            if (dataTypeName.ToUpper() == "JSONB")
                            {
                                writer.Write(dbCellVal, NpgsqlTypes.NpgsqlDbType.Jsonb);
                            }
                            else
                            {
                                writer.Write(dbCellVal, dataTypeName);
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
            await DbSession.OpenAsync();
            InsertImpl();
        }
    }
}
