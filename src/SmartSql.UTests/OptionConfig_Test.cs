using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartSql.Abstractions;
using SmartSql.Abstractions.Command;
using SmartSql.Abstractions.DataReaderDeserializer;
using SmartSql.Abstractions.DbSession;
using SmartSql.Command;
using SmartSql.DataReaderDeserializer;
using SmartSql.DbSession;
using SmartSql.Options;
using SmartSql.UTests.Entity;
using Xunit;

namespace SmartSql.UTests
{
    public class OptionConfig_Test : TestBase
    {
        private static IConfiguration Configuration;
        private IDataReaderDeserializerFactory _deserializerFactory;
        private IDbConnectionSessionStore _sessionStore;
        private ICommandExecuter _commandExecuter;
        private SmartSqlContext _smartSqlContext;
        private ISqlBuilder _sqlBuilder;

        public OptionConfig_Test()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json");

            Configuration = builder.Build();

            var services = new ServiceCollection();

            services.AddOptions();
            services.Configure<SmartSqlConfigOptions>(Configuration);
            var provider = services.BuildServiceProvider();
            var options = provider.GetService<IOptionsSnapshot<SmartSqlConfigOptions>>();

            var _configLoader = new OptionConfigLoader(options, LoggerFactory);
            var config = _configLoader.Load();
            _sessionStore = new DbConnectionSessionStore(LoggerFactory, DbProviderFactory);
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
    }
}