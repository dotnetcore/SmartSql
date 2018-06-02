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
        SmartSqlContext _smartSqlContext;
        public DapperDataReaderDeserializerFactory_Test()
        {
            _deserializerFactory = new DapperDataReaderDeserializerFactory();

            _sessionStore = new DbConnectionSessionStore(LoggerFactory, DbProviderFactory);
            var _configLoader = new LocalFileConfigLoader(SqlMapConfigFilePath, LoggerFactory);
            var config = _configLoader.Load();
            _smartSqlContext = new SmartSqlContext(LoggerFactory.CreateLogger<SmartSqlContext>(), config);
            var _sqlBuilder = new SqlBuilder(LoggerFactory.CreateLogger<SqlBuilder>(), _smartSqlContext);
            var _preparedCommand = new PreparedCommand(_sqlBuilder,  _smartSqlContext);
            _commandExecuter = new CommandExecuter(LoggerFactory.CreateLogger<CommandExecuter>(), _preparedCommand);
        }

        [Fact]
        public void Des()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "Query",
            };
            var dbSession = _sessionStore.CreateDbSession(DataSource);
            var result = _commandExecuter.ExecuteReader(dbSession, context);
            var deser = _deserializerFactory.Create();
            var users = deser.ToEnumerable<T_Entity>(context, result);
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
