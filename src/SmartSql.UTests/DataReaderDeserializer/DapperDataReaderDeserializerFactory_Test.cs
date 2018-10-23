using SmartSql.Abstractions.Command;
using SmartSql.Abstractions.DataReaderDeserializer;
using SmartSql.Abstractions.DbSession;
using SmartSql.DbSession;
using System;
using Xunit;
using Microsoft.Extensions.Logging;
using SmartSql.Command;
using SmartSql.Abstractions;
using System.Linq;
using SmartSql.DapperDeserializer;
using SmartSql.Abstractions.TypeHandler;
using SmartSql.UTests.Entity;

namespace SmartSql.UTests.DataReaderDeserializer
{
    public class DapperDataReaderDeserializerFactory_Test : TestBase, IDisposable
    {
        IDataReaderDeserializerFactory _deserializerFactory;
        IDbConnectionSessionStore _sessionStore;
        ICommandExecuter _commandExecuter;
        SmartSqlOptions _smartSqlOptions;
        public DapperDataReaderDeserializerFactory_Test()
        {
            _deserializerFactory = new DapperDataReaderDeserializerFactory();
            _smartSqlOptions = new SmartSqlOptions { DataReaderDeserializerFactory = _deserializerFactory };
            _smartSqlOptions.Setup();
            _sessionStore = _smartSqlOptions.DbSessionStore;
            _commandExecuter = new CommandExecuter(LoggerFactory.CreateLogger<CommandExecuter>(), _smartSqlOptions.PreparedCommand);
        }

        [Fact]
        public void Des()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "Query",
                Request = new { Taken = 10 }
            };
            context.Setup(_smartSqlOptions);
            var dbSession = _sessionStore.CreateDbSession(DataSource);
            var result = _commandExecuter.ExecuteReader(dbSession, context);
            var wrapper = new DataReaderWrapper(result);
            var deser = _deserializerFactory.Create();
            var users = deser.ToEnumerable<T_Entity>(context, wrapper);
            var list = users.ToList();
            result.Close();
            result.Dispose();
        }

        public void Dispose()
        {
            _sessionStore.Dispose();
        }
    }
}
