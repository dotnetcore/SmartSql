using System.Data;
using System.Data.Common;

namespace SmartSql.DbSession
{
    public interface ITransaction
    {
        DbTransaction BeginTransaction();
        DbTransaction BeginTransaction(IsolationLevel isolationLevel);
        void CommitTransaction();
        void RollbackTransaction();
    }
}