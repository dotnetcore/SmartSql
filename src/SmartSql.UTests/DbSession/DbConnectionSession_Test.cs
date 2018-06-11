using SmartSql.Abstractions.DataSource;
using SmartSql.Abstractions.DbSession;
using SmartSql.DbSession;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Xunit;

namespace SmartSql.UTests.DbSession
{
    public class DbConnectionSession_Test : TestBase, IDisposable
    {
        IDbConnectionSessionStore _sessionStore;
        IDbConnectionSession _connectionSession;
        public DbConnectionSession_Test()
        {
            _sessionStore = new DbConnectionSessionStore(LoggerFactory, DbProviderFactory);
            _connectionSession = _sessionStore.GetOrAddDbSession(DataSource);
        }
        [Fact]
        public void BeginTransaction()
        {
            _connectionSession.BeginTransaction();
            _connectionSession.RollbackTransaction();
        }
        [Fact]
        public void BeginTransactionAndIL()
        {
            IsolationLevel isolationLevel = IsolationLevel.Unspecified;
            _connectionSession.BeginTransaction(isolationLevel);
            _connectionSession.RollbackTransaction();
        }
        [Fact]
        public void CloseConnection()
        {
            _connectionSession.CloseConnection();
        }
        [Fact]
        public void CommitTransaction()
        {
            _connectionSession.BeginTransaction();
            _connectionSession.CommitTransaction();
        }

        public void Dispose()
        {
            _sessionStore.Dispose();
            //_connectionSession.Dispose();
        }
        [Fact]
        public void OpenConnection()
        {
            _connectionSession.OpenConnection();
        }
        [Fact]
        public void RollbackTransaction()
        {
            _connectionSession.BeginTransaction();
            _connectionSession.RollbackTransaction();
        }
    }
}
