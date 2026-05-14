using System.IO;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SmartSql.Options;
using Xunit;

namespace SmartSql.Test.Integration;

public class OptionConfigBuilderTests
{
    [Fact]
    public void Should_RegisterSmartSql_When_UsingOptionsConfig()
    {
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("SmartSqlMapConfig.json", false, true);

        var configuration = configBuilder.Build();
        var services = new ServiceCollection();
        services.AddOptions();
        var smartSqlConfigJson = configuration.GetSection("SmartSqlMapConfig");
        services.Configure<SmartSqlConfigOptions>("OptionConfigBuilderTests", smartSqlConfigJson);

        services.AddSmartSql((sp) =>
        {
            return new SmartSqlBuilder()
            .UseAlias("OptionConfigBuilderTests")
            .UseOptions(sp);
        });
        var serviceProvider = services.BuildServiceProvider();
        var sqlMapper = serviceProvider.GetRequiredService<ISqlMapper>();
    }
}
