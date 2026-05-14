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
            .WithResourceMapping(
                new FileInfo(Path.Combine("DB", "init-mysql-db.sql")),
                "/docker-entrypoint-initdb.d/init.sql")
            .Build();

        _redisContainer = new RedisBuilder("redis:7")
            .WithPortBinding(6379, 6379)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _mySqlContainer.StartAsync();
        await _redisContainer.StartAsync();

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
        DbSessionFactory = SmartSqlBuilder.DbSessionFactory;
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
    public IDbSessionFactory DbSessionFactory { get; private set; }
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
        SmartSqlBuilder.Dispose();
        await _redisContainer.DisposeAsync();
        await _mySqlContainer.DisposeAsync();
    }
}

[CollectionDefinition(SmartSqlFixture.GLOBAL_SMART_SQL)]
public class SmartSqlCollection : ICollectionFixture<SmartSqlFixture>;
