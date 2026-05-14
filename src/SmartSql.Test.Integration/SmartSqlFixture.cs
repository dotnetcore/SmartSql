using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using SmartSql.DbSession;
using SmartSql.DyRepository;
using SmartSql.Middlewares.Filters;
using SmartSql.Test.Entities;
using SmartSql.Test.Repositories;
using Testcontainers.MySql;
using Testcontainers.Redis;
using Xunit;

namespace SmartSql.Test.Integration;

public class SmartSqlFixture : IAsyncLifetime
{
    public const string GLOBAL_SMART_SQL = "GlobalSmartSql";

    private readonly MySqlContainer _mySqlContainer;
    private readonly RedisContainer _redisContainer;

    public SmartSqlFixture()
    {
        _mySqlContainer = new MySqlBuilder("mysql:8.0")
            .WithPortBinding(3306, 3306)
            .WithDatabase("SmartSqlTestDB")
            .WithUsername("root")
            .WithPassword("root")
            .Build();

        _redisContainer = new RedisBuilder("redis:7")
            .WithPortBinding(6379, 6379)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _mySqlContainer.StartAsync();
        await _redisContainer.StartAsync();
        await InitDatabaseAsync();

        LoggerFactory = new LoggerFactory(Enumerable.Empty<ILoggerProvider>(),
            new LoggerFilterOptions { MinLevel = LogLevel.Debug });
        var logPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "logs", "SmartSql.log");
        LoggerFactory.AddFile(logPath, LogLevel.Trace);

        SmartSqlBuilder = new SmartSqlBuilder()
            .UseXmlConfig()
            .UseLoggerFactory(LoggerFactory)
            .UseAlias(GLOBAL_SMART_SQL)
            .AddFilter<TestPrepareStatementFilter>()
            .RegisterEntity(typeof(AllPrimitive))
            .UseCUDConfigBuilder()
            .Build();
        SqlMapper = SmartSqlBuilder.SqlMapper;

        RepositoryBuilder = new EmitRepositoryBuilder(null, null,
            LoggerFactory.CreateLogger<EmitRepositoryBuilder>());
        RepositoryFactory = new RepositoryFactory(RepositoryBuilder,
            LoggerFactory.CreateLogger<RepositoryFactory>());
        UsedCacheRepository =
            RepositoryFactory.CreateInstance(typeof(IUsedCacheRepository), SqlMapper) as IUsedCacheRepository;
        AllPrimitiveRepository =
            RepositoryFactory.CreateInstance(typeof(IAllPrimitiveRepository), SqlMapper) as IAllPrimitiveRepository;
        UserRepository =
            RepositoryFactory.CreateInstance(typeof(IUserRepository), SqlMapper) as IUserRepository;
        ColumnAnnotationRepository =
            RepositoryFactory.CreateInstance(typeof(IColumnAnnotationRepository), SqlMapper) as IColumnAnnotationRepository;
        InitTestData();
    }

    private async Task InitDatabaseAsync()
    {
        var initSql = await File.ReadAllTextAsync(Path.Combine("DB", "init-mysql-db.sql"));
        // Remove the CREATE DATABASE line since Testcontainers already creates the database
        var tableSql = string.Join('\n', initSql.Split('\n')
            .Where(line => !line.TrimStart().StartsWith("CREATE DATABASE", StringComparison.OrdinalIgnoreCase)));
        await _mySqlContainer.ExecScriptAsync(tableSql);

        // Create stored procedure
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
        {
            throw new Exception($"Failed to create SP: {createSpResult.Stderr}");
        }
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

    public SmartSqlBuilder SmartSqlBuilder { get; private set; }
    public ISqlMapper SqlMapper { get; private set; }
    public ILoggerFactory LoggerFactory { get; private set; }
    public IRepositoryBuilder RepositoryBuilder { get; private set; }
    public IRepositoryFactory RepositoryFactory { get; private set; }

    public IUsedCacheRepository UsedCacheRepository { get; private set; }
    public IAllPrimitiveRepository AllPrimitiveRepository { get; private set; }
    public IUserRepository UserRepository { get; private set; }
    public IColumnAnnotationRepository ColumnAnnotationRepository { get; private set; }

    public async Task DisposeAsync()
    {
        SmartSqlBuilder?.Dispose();
        await _redisContainer.DisposeAsync();
        await _mySqlContainer.DisposeAsync();
    }
}

[CollectionDefinition(SmartSqlFixture.GLOBAL_SMART_SQL)]
public class SmartSqlCollection : ICollectionFixture<SmartSqlFixture>;
