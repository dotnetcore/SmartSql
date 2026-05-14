using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using SmartSql.Configuration;
using SmartSql.DataSource;
using SmartSql.Exceptions;
using Xunit;

namespace SmartSql.Test.Unit;

public class SmartSqlContainerTests : IDisposable
{
    private readonly List<string> _registeredAliases = new();

    private SmartSqlBuilder CreateBuilder(string alias)
    {
        var config = new SmartSqlConfig { LoggerFactory = NullLoggerFactory.Instance };
        var write = new WriteDataSource
        {
            Name = "Write",
            ConnectionString = "Data Source=:memory:",
            DbProvider = DbProviderManager.SQLITE_DBPROVIDER
        };
        config.Database = new Database
        {
            DbProvider = write.DbProvider,
            Write = write
        };

        var builder = new SmartSqlBuilder()
            .UseNativeConfig(config)
            .UseAlias(alias);

        _registeredAliases.Add(alias);
        return builder;
    }

    public void Dispose()
    {
        foreach (var alias in _registeredAliases)
        {
            var existing = SmartSqlContainer.Instance.GetSmartSql(alias);
            if (existing != null)
            {
                existing.Dispose();
            }
        }

        _registeredAliases.Clear();
    }

    [Fact]
    public void Should_GetBuilder_When_Registered()
    {
        var alias = $"test_register_{Guid.NewGuid():N}";
        var builder = CreateBuilder(alias);
        builder.Build();

        var result = SmartSqlContainer.Instance.GetSmartSql(alias);

        result.Should().NotBeNull();
        result.Should().BeSameAs(builder);
    }

    [Fact]
    public void Should_ReturnNull_When_GetNonexistentAlias()
    {
        var result = SmartSqlContainer.Instance.GetSmartSql($"nonexistent_{Guid.NewGuid():N}");

        result.Should().BeNull();
    }

    [Fact]
    public void Should_Throw_When_RegisterDuplicateAlias()
    {
        var alias = $"test_dup_{Guid.NewGuid():N}";
        var builder1 = CreateBuilder(alias);
        builder1.Build();

        var builder2 = CreateBuilder(alias);
        var act = () => builder2.Build();

        act.Should().Throw<SmartSqlException>()
            .WithMessage($"*{alias}*already exist*");
    }

    [Fact]
    public void Should_Throw_When_GetSmartSqlWithNullAlias()
    {
        var act = () => SmartSqlContainer.Instance.GetSmartSql(null);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("alias");
    }

    [Fact]
    public void Should_DisposeAllBuilders_When_DisposeCalled()
    {
        var alias1 = $"test_disp1_{Guid.NewGuid():N}";
        var alias2 = $"test_disp2_{Guid.NewGuid():N}";

        var builder1 = CreateBuilder(alias1);
        builder1.Build();

        var builder2 = CreateBuilder(alias2);
        builder2.Build();

        SmartSqlContainer.Instance.Dispose();

        SmartSqlContainer.Instance.GetSmartSql(alias1).Should().BeNull();
        SmartSqlContainer.Instance.GetSmartSql(alias2).Should().BeNull();

        _registeredAliases.Clear();
    }
}
