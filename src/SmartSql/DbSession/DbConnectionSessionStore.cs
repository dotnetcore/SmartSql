using Microsoft.Extensions.Logging;
using SmartSql.Abstractions;
using SmartSql.Abstractions.DataSource;
using SmartSql.Abstractions.DbSession;
using SmartSql.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading;

namespace SmartSql.DbSession
{
    /// <summary>
    /// DbConnection Session Store 
    /// </summary>
    public class DbConnectionSessionStore : IDbConnectionSessionStore
    {
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly DbProviderFactory _dbProviderFactory;
        private AsyncLocal<IDbConnectionSession> _staticSession = new AsyncLocal<IDbConnectionSession>();

        public DbConnectionSessionStore(ILoggerFactory loggerFactory
            , DbProviderFactory dbProviderFactory)
        {
            _logger = loggerFactory.CreateLogger<DbConnectionSessionStore>();
            _loggerFactory = loggerFactory;
            _dbProviderFactory = dbProviderFactory;
        }

        public IDbConnectionSession LocalSession
        {
            get
            {
                return _staticSession.Value;
            }
        }

        public void Store(IDbConnectionSession session)
        {
            if (_staticSession.Value == null)
            {
                _staticSession.Value = session;
            }
        }
        public IDbConnectionSession CreateDbSession(IDataSource dataSource)
        {
            ILogger<DbConnectionSession> dbSessionLogger = _loggerFactory.CreateLogger<DbConnectionSession>();
            IDbConnectionSession dbSession = new DbConnectionSession(dbSessionLogger, _dbProviderFactory, dataSource);
            Store(dbSession);
            return dbSession;
        }

        public IDbConnectionSession GetOrAddDbSession(IDataSource dataSource)
        {
            if (LocalSession != null) { return LocalSession; }
            return CreateDbSession(dataSource);
        }

        public void Dispose()
        {
            _staticSession.Value?.Dispose();
            _staticSession.Value = null;
        }
    }
}
