using SmartSql.Abstractions.Command;
using SmartSql.Abstractions.DataReaderDeserializer;
using SmartSql.Abstractions.DbSession;
using SmartSql.Abstractions.TypeHandler;
using SmartSql.DataReaderDeserializer;
using SmartSql.DbSession;
using System;
using Xunit;
using Microsoft.Extensions.Logging;
using SmartSql.Command;
using SmartSql.Abstractions;
using SmartSql.UTests.Entity;
using System.Linq;

namespace SmartSql.UTests.DataReaderDeserializer
{
    public class EmitDataReaderDeserializer_Test : TestBase, IDisposable
    {
        IDataReaderDeserializerFactory _deserializerFactory;
        IDbConnectionSessionStore _sessionStore;
        ICommandExecuter _commandExecuter;
        SmartSqlContext _smartSqlContext;
        ISqlBuilder _sqlBuilder;
        public EmitDataReaderDeserializer_Test()
        {
            _sessionStore = new DbConnectionSessionStore(LoggerFactory, DbProviderFactory);
            var _configLoader = new LocalFileConfigLoader(SqlMapConfigFilePath, LoggerFactory);
            var config = _configLoader.Load();
            _smartSqlContext = new SmartSqlContext(LoggerFactory.CreateLogger<SmartSqlContext>(), config);
            _sqlBuilder = new SqlBuilder(LoggerFactory.CreateLogger<SqlBuilder>(), _smartSqlContext);
            var _preparedCommand = new PreparedCommand(LoggerFactory.CreateLogger<PreparedCommand>(), _smartSqlContext);
            _commandExecuter = new CommandExecuter(LoggerFactory.CreateLogger<CommandExecuter>(), _preparedCommand);


            _deserializerFactory = new EmitDataReaderDeserializerFactory();
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
            context.Setup(_smartSqlContext, _sqlBuilder);
            var dbSession = _sessionStore.CreateDbSession(DataSource);
            var result = _commandExecuter.ExecuteReader(dbSession, context);
            var deser = _deserializerFactory.Create();
            var list = deser.ToEnumerable<T_Entity>(context, result).ToList();
            result.Close();
            result.Dispose();
        }
        [Fact]
        public void QueryStatus()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "QueryStatus",
            };
            context.Setup(_smartSqlContext, _sqlBuilder);
            var dbSession = _sessionStore.CreateDbSession(DataSource);
            var result = _commandExecuter.ExecuteReader(dbSession, context);
            var deser = _deserializerFactory.Create();
            var userIds = deser.ToEnumerable<EntityStatus>(context, result).ToList();
            result.Close();
            result.Dispose();
        }
        [Fact]
        public void QueryNullStatus()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "QueryNullStatus",
            };
            context.Setup(_smartSqlContext, _sqlBuilder);
            var dbSession = _sessionStore.CreateDbSession(DataSource);
            var result = _commandExecuter.ExecuteReader(dbSession, context);
            var deser = _deserializerFactory.Create();
            var userIds = deser.ToEnumerable<EntityStatus?>(context, result).ToList();
            result.Close();
            result.Dispose();
        }



        public void Dispose()
        {
            _sessionStore.Dispose();
        }
    }
}
