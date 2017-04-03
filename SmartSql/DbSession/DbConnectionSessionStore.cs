using SmartSql.Abstractions.DbSession;
using SmartSql.Abstractions.Logging;
using SmartSql.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;


namespace SmartSql.DbSession
{
    /// <summary>
    /// For 
    /// </summary>
    public class DbConnectionSessionStore : IDbConnectionSessionStore
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(DbConnectionSessionStore));
        const string KEY = "SmartSql-Local-DbSesstion-";
        protected string sessionName = string.Empty;
        [ThreadStatic]
        private static readonly Dictionary<string, IDbConnectionSession> staticSessions = new Dictionary<string, IDbConnectionSession>();
        public IDbConnectionSession LocalSession
        {
            get
            {
                if (staticSessions == null)
                {
                    _logger.Error($"SmartSql.DbConnectionSessionStore.LocalSession.staticSessions sessionName:{sessionName} is missing");
                    throw new SmartSqlException("SmartSql DbConnectionSessionStore.staticSessions is missing.");
                };
                staticSessions.TryGetValue(sessionName, out IDbConnectionSession session);
                return session;
            }
        }
        public DbConnectionSessionStore(String smartSqlMapperId)
        {
            sessionName = KEY + smartSqlMapperId;
        }
        public void Dispose()
        {
            staticSessions[sessionName] = null;
        }
        public void Store(IDbConnectionSession session)
        {
            staticSessions[sessionName] = session;
        }
    }
}
