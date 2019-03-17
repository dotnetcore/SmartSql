using SmartSql.DbSession;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SmartSql.Bulk.SqlServer
{
    public class BulkInsert : AbstractBulkInsert
    {
        public BulkInsert(IDbSession dbSession) : base(dbSession)
        {

        }
        public override void Insert()
        {
            DbSession.Open();
            var conn = DbSession.Connection as SqlConnection;
            using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(conn))
            {
                sqlBulkCopy.DestinationTableName = Table.TableName;
                sqlBulkCopy.WriteToServer(Table);
            }
        }
        public override async Task InsertAsync()
        {
            await DbSession.OpenAsync();
            var conn = DbSession.Connection as SqlConnection;
            using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(conn))
            {
                sqlBulkCopy.DestinationTableName = Table.TableName;
                await sqlBulkCopy.WriteToServerAsync(Table);
            }
        }
    }
}
