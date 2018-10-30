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
            MySqlBulkLoader bulkLoader = GetBulkLoader(conn);
            bulkLoader.Load();
        }
        private string _fieldTerminator = ",";
        private char _fieldQuotationCharacter = '"';
        private char _escapeCharacter = '"';
        private string _lineTerminator = "\r\n";
        public override async Task InsertAsync()
        {
            var dataSource = DataSourceFilter.Elect(new RequestContext { DataSourceChoice = DataSourceChoice.Write });
            var dbSession = SessionStore.GetOrAddDbSession(dataSource);
            await dbSession.OpenConnectionAsync();
            var conn = dbSession.Connection as MySqlConnection;
            MySqlBulkLoader bulkLoader = GetBulkLoader(conn);
            await bulkLoader.LoadAsync();
        }

        private MySqlBulkLoader GetBulkLoader(MySqlConnection conn)
        {
            return new MySqlBulkLoader(conn)
            {
                FieldTerminator = _fieldTerminator,
                FieldQuotationCharacter = _fieldQuotationCharacter,
                EscapeCharacter = _escapeCharacter,
                LineTerminator = _lineTerminator,
                FileName = ToCSV(),
                NumberOfLinesToSkip = 0,
                TableName = Table.Name,
            };
        }

        private string ToCSV()
        {
            InitColumnMappings();
            StringBuilder dataBuilder = new StringBuilder();
            foreach (var row in Table.Rows)
            {
                var colIndex = 0;
                foreach (var colMappingKey in ColumnMappings)
                {
                    var colMapping = colMappingKey.Value;
                    var col = Table.Columns[colMapping.Column];
                    if (colIndex != 0) dataBuilder.Append(_fieldTerminator);
                    if (col.DataType == typeof(string)
                        && row[col.Name] != null
                        && row[col.Name].ToString().Contains(_fieldTerminator))
                    {
                        dataBuilder.AppendFormat("\"{0}\"", row[col.Name].ToString().Replace("\"", "\"\""));
                    }
                    else
                    {
                        var colValStr = (col.AutoIncrement.HasValue && col.AutoIncrement.Value) ? "" : row[col.Name]?.ToString();
                        dataBuilder.Append(colValStr);
                    }
                    colIndex++;
                }
                dataBuilder.Append(_lineTerminator);
            }
            var fileName = Guid.NewGuid().ToString("N") + ".csv";
            fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            File.WriteAllText(fileName, dataBuilder.ToString());
            return fileName;
        }
    }
}
