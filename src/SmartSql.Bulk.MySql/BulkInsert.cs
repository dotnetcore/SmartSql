using MySql.Data.MySqlClient;
using SmartSql.DbSession;
using SmartSql.Reflection.TypeConstants;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MySqlConnector
namespace SmartSql.Bulk.MySqlConnector
#else
namespace SmartSql.Bulk.MySql
#endif
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

        public String SecureFilePriv { get; set; }
        public String DateTimeFormat { get; set; }
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
            var bulkLoader = new MySqlBulkLoader(conn)
            {
                FieldTerminator = _fieldTerminator,
                FieldQuotationCharacter = _fieldQuotationCharacter,
                EscapeCharacter = _escapeCharacter,
                LineTerminator = _lineTerminator,
                FileName = ToCSV(),
                NumberOfLinesToSkip = 0,
                TableName = Table.TableName
            };
            foreach (DataColumn dbCol in Table.Columns)
            {
                bulkLoader.Columns.Add(dbCol.ColumnName);
            }

            return bulkLoader;
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
                    else if (dataColumn.DataType == CommonType.DateTime || dataColumn.DataType == typeof(DateTime?))
                    {
                        var originCell = row[dataColumn];
                        if (originCell is DBNull)
                        {
                            dataBuilder.Append(DBNull.Value);
                        }
                        else
                        {
                            var dateCell = (DateTime) originCell;
                            var dateCellTime = dateCell.ToString(DateTimeFormat);
                            dataBuilder.Append(dateCellTime);
                        }
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
            var fileDir = SecureFilePriv ?? AppDomain.CurrentDomain.BaseDirectory;
            fileName = Path.Combine(fileDir, fileName);
            File.WriteAllText(fileName, dataBuilder.ToString());
            return fileName;
        }
    }
}