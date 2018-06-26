using SmartSql.Abstractions.DbSession;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.Abstractions
{
    public interface ITransaction
    {
        IDbConnectionSession BeginTransaction();
        IDbConnectionSession BeginTransaction(RequestContext context);
        IDbConnectionSession BeginTransaction(IsolationLevel isolationLevel);
        IDbConnectionSession BeginTransaction(RequestContext context, IsolationLevel isolationLevel);
        void CommitTransaction();
        void RollbackTransaction();
    }
}
