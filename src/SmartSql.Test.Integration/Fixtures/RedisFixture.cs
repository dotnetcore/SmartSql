using System.Threading.Tasks;
using Testcontainers.Redis;
using Xunit;

namespace SmartSql.Test.Integration.Fixtures;

public class RedisFixture : IAsyncLifetime
{
    public const string CollectionName = "Redis";
    private readonly RedisContainer _redisContainer;

    public RedisFixture()
    {
        _redisContainer = new RedisBuilder()
            .WithImage("redis:7")
            .Build();
    }

    public string ConnectionString => _redisContainer.GetConnectionString();

    public async Task InitializeAsync() => await _redisContainer.StartAsync();
    public async Task DisposeAsync() => await _redisContainer.DisposeAsync();
}

[CollectionDefinition(RedisFixture.CollectionName)]
public class RedisCollection : ICollectionFixture<RedisFixture>;
