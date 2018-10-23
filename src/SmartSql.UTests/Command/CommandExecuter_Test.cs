using SmartSql.Abstractions;
using SmartSql.Abstractions.Command;
using SmartSql.Abstractions.DbSession;
using SmartSql.DbSession;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Microsoft.Extensions.Logging;
using SmartSql.Command;
using Xunit;
using SmartSql.UTests.Entity;
using SmartSql.Abstractions.TypeHandler;

namespace SmartSql.UTests.Command
{
    public class CommandExecuter_Test : TestBase, IDisposable
    {
        SmartSqlOptions _smartSqlOptions;
        IDbConnectionSessionStore _sessionStore;
        ICommandExecuter _commandExecuter;
        public CommandExecuter_Test()
        {
            _smartSqlOptions = new SmartSqlOptions();
            _smartSqlOptions.Setup();
            _sessionStore = _smartSqlOptions.DbSessionStore;
            _commandExecuter = new CommandExecuter(LoggerFactory.CreateLogger<CommandExecuter>(), _smartSqlOptions.PreparedCommand);
        }

        public void Dispose()
        {
            _smartSqlOptions.DbSessionStore.Dispose();
        }
        [Fact]
        public void ExecuteNonQuery()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "Delete",
                Request = new { Id = 3 }
            };
            context.Setup(_smartSqlOptions);
            var dbSession = _sessionStore.CreateDbSession(DataSource);
            var result = _commandExecuter.ExecuteNonQuery(dbSession, context);
            //Assert.Equal<int>(1, result);
        }
        [Fact]
        public void ExecuteReader()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "Query",

                //Request = new { Id = 1, UserName = "SmartSql" },
            };
            context.Setup(_smartSqlOptions);
            var dbSession = _sessionStore.CreateDbSession(DataSource);
            var result = _commandExecuter.ExecuteReader(dbSession, context);
            while (result.Read())
            {
                var id = result.GetInt64(0);
            }
            result.Close();
            result.Dispose();
        }

        [Fact]
        public void ExecuteScalar()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "GetRecord",
                Request = new { Id = 2 }
            };
            context.Setup(_smartSqlOptions);
            var dbSession = _sessionStore.CreateDbSession(DataSource);
            var result = _commandExecuter.ExecuteScalar(dbSession, context);
            //Assert.Equal(1, result);
        }

        [Fact]
        public void ExecuteScalar_Add()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "Insert",
                Request = new T_Entity
                {
                    CreationTime = DateTime.Now,
                    FString = "SmartSql-" + this.GetHashCode(),
                    FBool = true,
                    FDecimal = 1,
                    FLong = 1,
                    FNullBool = null,
                    FNullDecimal = null,
                    LastUpdateTime = null,
                    NullStatus = null,
                    Status = EntityStatus.Ok
                }
            };
            context.Setup(_smartSqlOptions);
            var dbSession = _sessionStore.CreateDbSession(DataSource);
            var result = _commandExecuter.ExecuteScalar(dbSession, context);

        }
        [Fact]
        public void SessionEx_Add()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "Insert",
                Request = new T_Entity
                {
                    CreationTime = DateTime.Now,
                    FString = "SmartSql-" + Guid.NewGuid().ToString("N"),
                    FBool = true,
                    FDecimal = 1,
                    FLong = 1,
                    FNullBool = false,
                    FNullDecimal = 1,
                    LastUpdateTime = null,
                    NullStatus = EntityStatus.Ok,
                    Status = EntityStatus.Ok
                }
            };
            context.Setup(_smartSqlOptions);
            var dbSession = _sessionStore.GetOrAddDbSession(DataSource);
            dbSession.Begin();
            for (int i = 0; i < 10; i++)
            {
                var result0 = _commandExecuter.ExecuteScalar(dbSession, context);
            }

            dbSession.End();
        }
    }
}
