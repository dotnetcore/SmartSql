using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SmartSql.DataSource;
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

    protected String ConnectionString => "Data Source=.;Initial Catalog=SmartSqlTestDB;Integrated Security=True";

    [Fact]
    public void Should_ResolveServices_When_AddingSmartSqlByFunc()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddSmartSql(sp => new SmartSqlBuilder().UseAlias("AddSmartSqlByFunc")
            .UseDataSource(DbProvider.SQLSERVER, ConnectionString));
        var serviceProvider = services.BuildServiceProvider();
        GetSmartSqlService(serviceProvider);
    }

    [Fact]
    public void Should_ResolveServices_When_AddingSmartSqlByAction()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddSmartSql((sp, smartsqlBuilder) => { smartsqlBuilder.UseAlias("AddSmartSqlByAction"); });
        var serviceProvider = services.BuildServiceProvider();
        GetSmartSqlService(serviceProvider);
    }

    [Fact]
    public void Should_ResolveRepository_When_AddingFromAssembly()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddSmartSql()
            .AddRepositoryFromAssembly(o =>
            {
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
        IServiceCollection services = new ServiceCollection();
        services.AddSmartSql("AddRepositoryFromAssemblyUseAlias")
            .AddRepositoryFromAssembly(o =>
            {
                o.SmartSqlAlias = "AddRepositoryFromAssemblyUseAlias";
                o.AssemblyString = "SmartSql.Test";
                o.Filter = (type) => { return type.Namespace == "SmartSql.Test.Repositories"; };
            });
        var serviceProvider = services.BuildServiceProvider();
        GetSmartSqlService(serviceProvider);
        IAllPrimitiveRepository allPrimitiveRepository =
            serviceProvider.GetRequiredService<IAllPrimitiveRepository>();
    }
}
