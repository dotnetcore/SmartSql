using SmartSql.Abstractions;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SmartSql.Batch.SqlServer
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
            var conn = dbSession.Connection as SqlConnection;
            using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(conn))
            {
                sqlBulkCopy.DestinationTableName = Table.Name;
                var dataTable = Table.ToDataTable();
                sqlBulkCopy.WriteToServer(dataTable);
            }
        }
        public override async Task InsertAsync()
        {
            var dataSource = DataSourceFilter.Elect(new RequestContext { DataSourceChoice = DataSourceChoice.Write });
            var dbSession = SessionStore.GetOrAddDbSession(dataSource);
            await dbSession.OpenConnectionAsync();
            var conn = dbSession.Connection as SqlConnection;
            using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(conn))
            {
                sqlBulkCopy.DestinationTableName = Table.Name;
                var dataTable = Table.ToDataTable();
                await sqlBulkCopy.WriteToServerAsync(dataTable);
            }
        }
    }
}
