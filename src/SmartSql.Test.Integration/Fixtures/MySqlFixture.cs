using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SmartSql.DyRepository;
using SmartSql.Middlewares.Filters;
using SmartSql.Test.Entities;
using SmartSql.Test.Repositories;
using Testcontainers.MySql;
using Xunit;

namespace SmartSql.Test.Integration.Fixtures;

public class MySqlFixture : IDbTestFixture
{
    public const string ALIAS = "MySqlIntegrationTest";
    public const string CollectionName = "MySql";

    private readonly MySqlContainer _mySqlContainer;

    public MySqlFixture()
    {
        _mySqlContainer = new MySqlBuilder("mysql:8.0")
            .WithDatabase("SmartSqlTestDB")
            .WithUsername("root")
            .WithPassword("root")
            .Build();
    }

    public ISqlMapper SqlMapper { get; private set; }
    public SmartSqlBuilder SmartSqlBuilder { get; private set; }
    public string DbProvider => DataSource.DbProvider.MYSQL;
    public ILoggerFactory LoggerFactory { get; private set; }
    public IRepositoryFactory RepositoryFactory { get; private set; }
    public IAllPrimitiveRepository AllPrimitiveRepository { get; private set; }
    public IUserRepository UserRepository { get; private set; }

    public async Task InitializeAsync()
    {
        await _mySqlContainer.StartAsync();
        await InitDatabaseAsync();
        BuildSmartSql();
        InitTestData();
    }

    private async Task InitDatabaseAsync()
    {
        var initSql = await File.ReadAllTextAsync(Path.Combine("DB", "init-mysql-db.sql"));
        var tableSql = string.Join('\n', initSql.Split('\n')
            .Where(line => !line.TrimStart().StartsWith("CREATE DATABASE", StringComparison.OrdinalIgnoreCase)));
        await _mySqlContainer.ExecScriptAsync(tableSql);

        var createSpResult = await _mySqlContainer.ExecAsync(
            new List<string> { "sh", "-c",
                @"mysql -uroot -proot SmartSqlTestDB <<'EOF'
DELIMITER //
CREATE PROCEDURE SP_Query(out Total int)
BEGIN
    Select Count(*) into Total From T_AllPrimitive T;
    SELECT T.* From T_AllPrimitive T limit 10;
END //
DELIMITER ;
EOF" });
        if (createSpResult.ExitCode != 0)
            throw new Exception($"Failed to create SP: {createSpResult.Stderr}");
    }

    private void BuildSmartSql()
    {
        var connectionString = $"server={_mySqlContainer.Hostname};port={_mySqlContainer.GetMappedPublicPort(3306)};uid=root;pwd=root;database=SmartSqlTestDB";
        LoggerFactory = new LoggerFactory(Enumerable.Empty<ILoggerProvider>(),
            new LoggerFilterOptions { MinLevel = LogLevel.Debug });
        var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "SmartSql-MySql.log");
        LoggerFactory.AddFile(logPath, LogLevel.Trace);

        SmartSqlBuilder = new SmartSqlBuilder()
            .UseXmlConfig()
            .UseDataSource(SmartSql.DataSource.DbProvider.MYSQL, connectionString)
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
        await _mySqlContainer.DisposeAsync();
    }
}

[CollectionDefinition(MySqlFixture.CollectionName)]
public class MySqlCollection : ICollectionFixture<MySqlFixture>;
