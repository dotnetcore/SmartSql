using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SmartSql.DyRepository;
using SmartSql.Middlewares.Filters;
using SmartSql.Test.Entities;
using SmartSql.Test.Repositories;
using Testcontainers.MsSql;
using Xunit;

namespace SmartSql.Test.Integration.Fixtures;

public class SqlServerFixture : IDbTestFixture
{
    public const string ALIAS = "SqlServerIntegrationTest";
    public const string CollectionName = "SqlServer";

    private readonly MsSqlContainer _sqlServerContainer;

    public SqlServerFixture()
    {
        _sqlServerContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .Build();
    }

    public ISqlMapper SqlMapper { get; private set; }
    public SmartSqlBuilder SmartSqlBuilder { get; private set; }
    public string DbProvider => DataSource.DbProvider.SQLSERVER;
    public ILoggerFactory LoggerFactory { get; private set; }
    public IRepositoryFactory RepositoryFactory { get; private set; }
    public IAllPrimitiveRepository AllPrimitiveRepository { get; private set; }
    public IUserRepository UserRepository { get; private set; }

    public async Task InitializeAsync()
    {
        await _sqlServerContainer.StartAsync();
        await InitDatabaseAsync();
        BuildSmartSql();
        InitTestData();
    }

    private async Task InitDatabaseAsync()
    {
        var initSql = await File.ReadAllTextAsync(Path.Combine("DB", "init-sqlserver-db.sql"));
        await _sqlServerContainer.ExecScriptAsync(initSql);
    }

    private void BuildSmartSql()
    {
        var connectionString = _sqlServerContainer.GetConnectionString();
        LoggerFactory = new LoggerFactory(Enumerable.Empty<ILoggerProvider>(),
            new LoggerFilterOptions { MinLevel = LogLevel.Debug });
        var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "SmartSql-SqlServer.log");
        LoggerFactory.AddFile(logPath, LogLevel.Trace);

        SmartSqlBuilder = new SmartSqlBuilder()
            .UseXmlConfig()
            .UseDataSource(DataSource.DbProvider.SQLSERVER, connectionString)
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
        await _sqlServerContainer.DisposeAsync();
    }
}

[CollectionDefinition(SqlServerFixture.CollectionName)]
public class SqlServerCollection : ICollectionFixture<SqlServerFixture>;
