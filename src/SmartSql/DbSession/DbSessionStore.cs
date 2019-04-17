using System.Threading;

namespace SmartSql.DbSession
{
    public class DbSessionStore : IDbSessionStore
    {
        private readonly AsyncLocal<IDbSession> _staticSession = new AsyncLocal<IDbSession>();
        private readonly IDbSessionFactory _dbSessionFactory;
        public DbSessionStore(IDbSessionFactory dbSessionFactory)
        {
            _dbSessionFactory = dbSessionFactory;
        }
        public IDbSession LocalSession => _staticSession.Value;

        public IDbSession Open()
        {
            if (LocalSession == null)
            {
                _staticSession.Value = _dbSessionFactory.Open();
            }
            return LocalSession;
        }

        public void Dispose()
        {
            _staticSession.Value?.Dispose();
            _staticSession.Value = null;
        }
    }
}
