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
        SmartSqlOptions _smartSqlOptions;
        IDbConnectionSessionStore _sessionStore;
        IPreparedCommand _preparedCommand;
        public PreparedCommand_Test()
        {
            _smartSqlOptions = new SmartSqlOptions();
            _smartSqlOptions.Setup();
            _sessionStore = _smartSqlOptions.DbSessionStore;
            _preparedCommand = new PreparedCommand(LoggerFactory.CreateLogger<PreparedCommand>(), _smartSqlOptions.SmartSqlContext);
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
            context.Setup(_smartSqlOptions);
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
