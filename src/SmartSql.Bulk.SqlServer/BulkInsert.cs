using SmartSql.DbSession;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SmartSql.Bulk.SqlServer
{
    public class BulkInsert : AbstractBulkInsert
    {
        public SqlBulkCopyOptions Options { get; set; } = SqlBulkCopyOptions.Default;
        public BulkInsert(IDbSession dbSession) : base(dbSession)
        {

        }
        private SqlBulkCopy CreateSqlBulkCopy()
        {
            var conn = DbSession.Connection as SqlConnection;
            return new SqlBulkCopy(conn, Options, DbSession.Transaction as SqlTransaction);
        }
        public override void Insert()
        {
            DbSession.Open();
            using (var sqlBulkCopy = CreateSqlBulkCopy())
            {
                sqlBulkCopy.DestinationTableName = Table.TableName;
                sqlBulkCopy.WriteToServer(Table);
            }
        }

        public override async Task InsertAsync()
        {
            await DbSession.OpenAsync();
            using (var sqlBulkCopy = CreateSqlBulkCopy())
            {
                sqlBulkCopy.DestinationTableName = Table.TableName;
                await sqlBulkCopy.WriteToServerAsync(Table);
            }
        }
    }
}
