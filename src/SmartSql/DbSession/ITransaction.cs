using System.Data;

namespace SmartSql.DbSession
{
    public interface ITransaction
    {
        void BeginTransaction();
        void BeginTransaction(IsolationLevel isolationLevel);
        void CommitTransaction();
        void RollbackTransaction();
    }
}