using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using SmartSql;
using SmartSql.DyRepository;
using SmartSql.Middlewares.Filters;
using SmartSql.Test.Entities;
using SmartSql.Test.Repositories;
using Xunit;

namespace SmartSql.Test.Integration.Fixtures;

public class SqliteFixture : IDbTestFixture
{
    public const string ALIAS = "SqliteIntegrationTest";
    public const string CollectionName = "Sqlite";

    private SqliteConnection _keepAliveConnection;

    public ISqlMapper SqlMapper { get; private set; }
    public SmartSqlBuilder SmartSqlBuilder { get; private set; }
    public string DbProvider => DataSource.DbProvider.SQLITE;
    public ILoggerFactory LoggerFactory { get; private set; }
    public IRepositoryFactory RepositoryFactory { get; private set; }
    public IAllPrimitiveRepository AllPrimitiveRepository { get; private set; }
    public IUserRepository UserRepository { get; private set; }

    public async Task InitializeAsync()
    {
        LoggerFactory = new LoggerFactory(Enumerable.Empty<ILoggerProvider>(),
            new LoggerFilterOptions { MinLevel = LogLevel.Debug });

        SmartSqlBuilder = new SmartSqlBuilder()
            .UseDataSource(DataSource.DbProvider.SQLITE, "Data Source=:memory:;Cache=Shared")
            .UseLoggerFactory(LoggerFactory)
            .UseAlias(ALIAS)
            .AddFilter<TestPrepareStatementFilter>()
            .RegisterEntity(typeof(AllPrimitive))
            .UseCUDConfigBuilder()
            .Build();
        SqlMapper = SmartSqlBuilder.SqlMapper;

        _keepAliveConnection = new SqliteConnection("Data Source=:memory:;Cache=Shared");
        _keepAliveConnection.Open();

        InitDatabase();
        BuildRepositories();
        InitTestData();
        await Task.CompletedTask;
    }

    private void InitDatabase()
    {
        var initSql = File.ReadAllText(Path.Combine("DB", "init-sqlite-db.sql"));
        foreach (var sql in initSql.Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim()).Where(s => s.Length > 0))
        {
            SqlMapper.ExecuteScalar<int>(new RequestContext { RealSql = sql });
        }
    }

    private void BuildRepositories()
    {
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
        _keepAliveConnection?.Close();
        _keepAliveConnection?.Dispose();
        await Task.CompletedTask;
    }
}

[CollectionDefinition(SqliteFixture.CollectionName)]
public class SqliteCollection : ICollectionFixture<SqliteFixture>;
