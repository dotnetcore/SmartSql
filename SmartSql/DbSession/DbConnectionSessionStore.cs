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
        const string KEY = "SmartSql-Local-DbSesstion-";
        protected string _sessionName = string.Empty;
        private static AsyncLocal<IDictionary<string, IDbConnectionSession>> staticSessions
            = new AsyncLocal<IDictionary<string, IDbConnectionSession>>();
        public DbConnectionSessionStore(ILoggerFactory loggerFactory
            , DbProviderFactory dbProviderFactory
            , String smartSqlMapperId)
        {
            _logger = loggerFactory.CreateLogger<DbConnectionSessionStore>();
            _sessionName = KEY + smartSqlMapperId;
            _loggerFactory = loggerFactory;
            _dbProviderFactory = dbProviderFactory;
        }

        public IDbConnectionSession LocalSession
        {
            get
            {
                if (staticSessions.Value == null)
                {
                    return null;
                };
                staticSessions.Value.TryGetValue(_sessionName, out IDbConnectionSession session);
                return session;
            }
        }

        public void Store(IDbConnectionSession session)
        {
            if (staticSessions.Value == null)
            {
                staticSessions.Value = new Dictionary<String, IDbConnectionSession>();
            }
            staticSessions.Value[_sessionName] = session;
        }
        public IDbConnectionSession CreateDbSession(IDataSource dataSource)
        {
            ILogger<DbConnectionSession> dbSessionLogger = _loggerFactory.CreateLogger<DbConnectionSession>();
            IDbConnectionSession session = new DbConnectionSession(dbSessionLogger, _dbProviderFactory, dataSource);
            session.CreateConnection();
            Store(session);
            return session;
        }

        public IDbConnectionSession GetOrAddDbSession(IDataSource dataSource)
        {
            if (LocalSession != null) { return LocalSession; }
            return CreateDbSession(dataSource);
        }

        public void Dispose()
        {
            if (staticSessions.Value != null)
            {
                staticSessions.Value[_sessionName].Dispose();
                staticSessions.Value[_sessionName] = null;
            }
        }
    }
}
