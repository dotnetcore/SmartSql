using MySql.Data.MySqlClient;
using SmartSql.DbSession;
using SmartSql.Reflection.TypeConstants;
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.Bulk.MySql
{
    public class BulkInsert : AbstractBulkInsert
    {
        public BulkInsert(IDbSession dbSession) : base(dbSession)
        {

        }

        public override void Insert()
        {
            DbSession.Open();
            var conn = DbSession.Connection as MySqlConnection;
            MySqlBulkLoader bulkLoader = GetBulkLoader(conn);
            bulkLoader.Load();
        }
        private string _fieldTerminator = ",";
        private char _fieldQuotationCharacter = '"';
        private char _escapeCharacter = '"';
        private string _lineTerminator = "\r\n";
        public override async Task InsertAsync()
        {
            await DbSession.OpenAsync();
            var conn = DbSession.Connection as MySqlConnection;
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
                TableName = Table.TableName,
            };
        }

        private string ToCSV()
        {
            StringBuilder dataBuilder = new StringBuilder();
            foreach (DataRow row in Table.Rows)
            {
                var colIndex = 0;
                foreach (DataColumn dataColumn in Table.Columns)
                {
                    if (colIndex != 0) dataBuilder.Append(_fieldTerminator);
                    
                    if (dataColumn.DataType == CommonType.String
                        && !row.IsNull(dataColumn)
                        && row[dataColumn].ToString().Contains(_fieldTerminator))
                    {
                        dataBuilder.AppendFormat("\"{0}\"", row[dataColumn].ToString().Replace("\"", "\"\""));
                    }
                    else
                    {
                        var colValStr = dataColumn.AutoIncrement ? "" : row[dataColumn]?.ToString();
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
