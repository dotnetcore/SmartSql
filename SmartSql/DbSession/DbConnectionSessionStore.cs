using Microsoft.Extensions.Logging;
using SmartSql.Abstractions.DbSession;
using SmartSql.Exceptions;
using System;
using System.Collections.Generic;
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
        const string KEY = "SmartSql-Local-DbSesstion-";
        protected string sessionName = string.Empty;
        private static AsyncLocal<IDictionary<string, IDbConnectionSession>> staticSessions
            = new AsyncLocal<IDictionary<string, IDbConnectionSession>>();
        public DbConnectionSessionStore(ILoggerFactory loggerFactory, String smartSqlMapperId)
        {
            _logger = loggerFactory.CreateLogger<DbConnectionSessionStore>();
            sessionName = KEY + smartSqlMapperId;
        }

        public IDbConnectionSession LocalSession
        {
            get
            {
                if (staticSessions.Value == null)
                {
                    return null;
                };
                staticSessions.Value.TryGetValue(sessionName, out IDbConnectionSession session);
                return session;
            }
        }
        public void Dispose()
        {
            if (staticSessions.Value != null)
            {
                staticSessions.Value[sessionName].Dispose();
                staticSessions.Value[sessionName] = null;
            }
        }

        public void Store(IDbConnectionSession session)
        {
            if (staticSessions.Value == null)
            {
                staticSessions.Value = new Dictionary<String, IDbConnectionSession>();
            }
            staticSessions.Value[sessionName] = session;
        }
    }
}
