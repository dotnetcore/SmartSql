using SmartSql.DbSession;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace SmartSql.Bulk.Oracle
{
    public class BulkInsert : AbstractBulkInsert
    {
        public BulkInsert(IDbSession dbSession) : base(dbSession)
        {
        }


        private void InsertImpl()
        {
            var conn = DbSession.Connection as OracleConnection;

            using (OracleTransaction transaction = conn.BeginTransaction())
            {
                //创建 OracleBulkCopy 对象，并指定数据库连接信息 
                using (OracleBulkCopy bulkCopy = new OracleBulkCopy(conn))
                {
                    //数据库表名称
                    bulkCopy.DestinationTableName = this.Table.TableName;
                    //指定批量插入的行数 
                    bulkCopy.BatchSize = this.Table.Rows.Count;

                    //指定 DataTable 和数据表的列名映射关系
                    for (int i = 0; i < this.Table.Columns.Count; i++)
                    {
                        bulkCopy.ColumnMappings.Add(this.Table.Columns[i].ColumnName, this.Table.Columns[i].ColumnName);
                    }
                    try
                    {
                        //将数据源添加到 OracleBulkCopy 对象中
                        bulkCopy.WriteToServer(this.Table);
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public override void Insert()
        {
            DbSession.Open();
            InsertImpl();
        }

        public override async Task InsertAsync()
        {
            await DbSession.OpenAsync();
            InsertImpl();
        }
    }
}
