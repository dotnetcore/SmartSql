using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SmartSql;
using SmartSql.ConfigBuilder;
using SmartSql.DyRepository;
using SmartSql.Middlewares.Filters;
using SmartSql.Test.Entities;
using SmartSql.Test.Repositories;
using Testcontainers.PostgreSql;
using Xunit;

namespace SmartSql.Test.Integration.Fixtures;

public class PostgreSqlFixture : IDbTestFixture
{
    public const string ALIAS = "PgIntegrationTest";
    public const string CollectionName = "PostgreSql";

    private readonly PostgreSqlContainer _pgContainer;

    public PostgreSqlFixture()
    {
        _pgContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithPortBinding(5432, true)
            .WithDatabase("SmartSqlTestDB")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();
    }

    public ISqlMapper SqlMapper { get; private set; }
    public SmartSqlBuilder SmartSqlBuilder { get; private set; }
    public string DbProvider => DataSource.DbProvider.POSTGRESQL;
    public ILoggerFactory LoggerFactory { get; private set; }
    public IRepositoryFactory RepositoryFactory { get; private set; }
    public IAllPrimitiveRepository AllPrimitiveRepository { get; private set; }
    public IUserRepository UserRepository { get; private set; }

    public async Task InitializeAsync()
    {
        await _pgContainer.StartAsync();
        await InitDatabaseAsync();
        BuildSmartSql();
        InitTestData();
    }

    private async Task InitDatabaseAsync()
    {
        var initSql = await File.ReadAllTextAsync(Path.Combine("DB", "init-postgresql-db.sql"));
        await _pgContainer.ExecScriptAsync(initSql);
    }

    private void BuildSmartSql()
    {
        var connectionString = _pgContainer.GetConnectionString();
        LoggerFactory = new LoggerFactory(Enumerable.Empty<ILoggerProvider>(),
            new LoggerFilterOptions { MinLevel = LogLevel.Debug });
        var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "SmartSql-Pg.log");
        LoggerFactory.AddFile(logPath, LogLevel.Trace);

        SmartSqlBuilder = new SmartSqlBuilder()
            .UseXmlConfig(ResourceType.File, "SmartSqlMapConfig.PostgreSql.xml")
            .UseDatabase(DataSource.DbProvider.POSTGRESQL, _pgContainer.GetConnectionString())
            .UseLoggerFactory(LoggerFactory)
            .UseAlias(ALIAS)
            .AddFilter<TestPrepareStatementFilter>()
            .RegisterEntity(typeof(AllPrimitive))
            .UseCUDConfigBuilder()
            .Build();
        SqlMapper = SmartSqlBuilder.SqlMapper;

        var repositoryBuilder = new EmitRepositoryBuilder(null, null,
            LoggerFactory.CreateLogger<EmitRepositoryBuilder>());
        RepositoryFactory = new RepositoryFactory(repositoryBuilder,
            LoggerFactory.CreateLogger<RepositoryFactory>());
        AllPrimitiveRepository =
            RepositoryFactory.CreateInstance(typeof(IAllPrimitiveRepository), SqlMapper) as IAllPrimitiveRepository;
        UserRepository =
            RepositoryFactory.CreateInstance(typeof(IUserRepository), SqlMapper) as IUserRepository;
    }

    private void InitTestData()
    {
        AllPrimitiveRepository.Truncate();
        for (int i = 0; i < 10; i++)
        {
            AllPrimitiveRepository.Insert(new AllPrimitive
            {
                NumericalEnum = i % 2 == 0 ? NumericalEnum.One : NumericalEnum.Two
            });
        }
    }

    public async Task DisposeAsync()
    {
        SmartSqlBuilder?.Dispose();
        await _pgContainer.DisposeAsync();
    }
}

[CollectionDefinition(PostgreSqlFixture.CollectionName)]
public class PgCollection : ICollectionFixture<PostgreSqlFixture>;
