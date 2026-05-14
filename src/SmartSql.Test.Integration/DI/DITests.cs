using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SmartSql.DbSession;
using SmartSql.Test.Repositories;
using Xunit;

namespace SmartSql.Test.Integration.DI;

public class DITests
{
    [Fact]
    public void Should_ResolveServices_When_AddingSmartSql()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddSmartSql("AddSmartSql");
        var serviceProvider = services.BuildServiceProvider();
        GetSmartSqlService(serviceProvider);
    }

    private void GetSmartSqlService(IServiceProvider serviceProvider)
    {
        var smartSqlBuilder = serviceProvider.GetRequiredService<SmartSqlBuilder>();
        var sqlMapper = serviceProvider.GetRequiredService<ISqlMapper>();
        var dbSessionFactory = serviceProvider.GetRequiredService<IDbSessionFactory>();
    }

    [Fact]
    public void Should_ResolveServices_When_AddingSmartSqlByFunc()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddSmartSql(sp => new SmartSqlBuilder().UseAlias("AddSmartSqlByFunc")
            .UseXmlConfig().UseCache(false));
        var serviceProvider = services.BuildServiceProvider();
        GetSmartSqlService(serviceProvider);
    }

    [Fact]
    public void Should_ResolveServices_When_AddingSmartSqlByAction()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddSmartSql((sp, smartsqlBuilder) =>
        {
            smartsqlBuilder.UseAlias("AddSmartSqlByAction");
        });
        var serviceProvider = services.BuildServiceProvider();
        GetSmartSqlService(serviceProvider);
    }

    [Fact]
    public void Should_ResolveRepository_When_AddingFromAssembly()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddSmartSql("AddSmartSqlFromAssembly")
            .AddRepositoryFromAssembly(o =>
            {
                o.SmartSqlAlias = "AddSmartSqlFromAssembly";
                o.AssemblyString = "SmartSql.Test";
                o.Filter = (type) => { return type.Namespace == "SmartSql.Test.Repositories"; };
            });
        var serviceProvider = services.BuildServiceProvider();
        GetSmartSqlService(serviceProvider);
        IAllPrimitiveRepository allPrimitiveRepository =
            serviceProvider.GetRequiredService<IAllPrimitiveRepository>();
    }

    [Fact]
    public void Should_ResolveRepository_When_AddingFromAssemblyWithAlias()
    {
        const string alias = "AddRepositoryFromAssemblyUseAlias";
        IServiceCollection services = new ServiceCollection();
        services.AddSmartSql(alias)
            .AddRepositoryFromAssembly(o =>
            {
                o.SmartSqlAlias = alias;
                o.AssemblyString = "SmartSql.Test";
                o.Filter = (type) => { return type.Namespace == "SmartSql.Test.Repositories"; };
            });
        var serviceProvider = services.BuildServiceProvider();
        GetSmartSqlService(serviceProvider);
        IAllPrimitiveRepository allPrimitiveRepository =
            serviceProvider.GetRequiredService<IAllPrimitiveRepository>();
    }
}
