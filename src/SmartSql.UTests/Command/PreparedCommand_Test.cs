using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SmartSql.Abstractions;
using SmartSql.Abstractions.Command;
using SmartSql.Abstractions.DbSession;
using SmartSql.Command;
using SmartSql.DbSession;
using Microsoft.Extensions.Logging;
using Xunit;
using SmartSql.Abstractions.TypeHandler;

namespace SmartSql.UTests.Command
{
    public class PreparedCommand_Test : TestBase, IDisposable
    {
        IDbConnectionSessionStore _sessionStore;
        IPreparedCommand _preparedCommand;
        SmartSqlContext _smartSqlContext;
        ISqlBuilder _sqlBuilder;
        public PreparedCommand_Test()
        {
            _sessionStore = new DbConnectionSessionStore(LoggerFactory, DbProviderFactory);
            var _configLoader = new LocalFileConfigLoader(SqlMapConfigFilePath, LoggerFactory);
            var config = _configLoader.Load();
            _smartSqlContext = new SmartSqlContext(LoggerFactory.CreateLogger<SmartSqlContext>(), config);

            _sqlBuilder = new SqlBuilder(LoggerFactory.CreateLogger<SqlBuilder>(), _smartSqlContext);

            _preparedCommand = new PreparedCommand(_smartSqlContext);
        }
        [Fact]
        public void Prepare()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "Query",
                Request = new { Id = 1, UserName = "SmartSql", Ids = new long[] { 1, 2, 3, 4 } }
            };
            context.Setup(_smartSqlContext, _sqlBuilder);
            var dbSession = _sessionStore.GetOrAddDbSession(DataSource);
            var dbCommand = _preparedCommand.Prepare(dbSession, context);

            Assert.NotNull(dbCommand);
        }

        public void Dispose()
        {
            _sessionStore.Dispose();
        }
    }
}
