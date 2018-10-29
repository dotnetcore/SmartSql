using MySql.Data.MySqlClient;
using SmartSql.Abstractions;
using SmartSql.Abstractions.DataSource;
using SmartSql.Abstractions.DbSession;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.Batch.MySql
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
            var conn = dbSession.Connection as MySqlConnection;
            var bulkLoader = new MySqlBulkLoader(conn)
            {
                FieldTerminator = ",",
                FieldQuotationCharacter = '"',
                EscapeCharacter = '"',
                LineTerminator = "\r\n",
                FileName = ToCSV(),
                NumberOfLinesToSkip = 0,
                TableName = Table.Name,
            };
            bulkLoader.Load();
        }
        public override async Task InsertAsync()
        {
            var dataSource = DataSourceFilter.Elect(new RequestContext { DataSourceChoice = DataSourceChoice.Write });
            var dbSession = SessionStore.GetOrAddDbSession(dataSource);
            await dbSession.OpenConnectionAsync();
            var conn = dbSession.Connection as MySqlConnection;
            var bulkLoader = new MySqlBulkLoader(conn)
            {
                FieldTerminator = ",",
                FieldQuotationCharacter = '"',
                EscapeCharacter = '"',
                LineTerminator = "\r\n",
                FileName = ToCSV(),
                NumberOfLinesToSkip = 0,
                TableName = Table.Name,
            };
            await bulkLoader.LoadAsync();
        }

        private string ToCSV()
        {
            StringBuilder dataBuilder = new StringBuilder();
            foreach (var row in Table.Rows)
            {
                for (int i = 0; i < Table.Columns.Count; i++)
                {
                    var col = Table.Columns[i];
                    if (i != 0) dataBuilder.Append(",");
                    if (col.DataType == typeof(string) && row[col.Name].ToString().Contains(","))
                    {
                        dataBuilder.Append("\"" + row[col.Name].ToString().Replace("\"", "\"\"") + "\"");
                    }
                    else dataBuilder.Append(row[col.Name]?.ToString());
                }
                dataBuilder.AppendLine();
            }
            var fileName = Guid.NewGuid().ToString("N") + ".csv";
            fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            File.WriteAllText(fileName, dataBuilder.ToString());
            return fileName;
        }
    }
}
