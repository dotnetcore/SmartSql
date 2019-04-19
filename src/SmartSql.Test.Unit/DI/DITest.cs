using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmartSql.DataSource;
using SmartSql.DbSession;
using SmartSql.Test.Repositories;
using SmartSql.Test.Unit.DyRepository;
using Xunit;

namespace SmartSql.Test.Unit.DI
{
    public class DITest
    {
        [Fact]
        public void AddSmartSql()
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
        public void AddSmartSql_Func()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSmartSql(sp =>
            {
                return new SmartSqlBuilder().UseAlias("AddSmartSql_Func").UseDataSource(DbProvider.SQLSERVER, ConnectionString);
            });
            var serviceProvider = services.BuildServiceProvider();
            GetSmartSqlService(serviceProvider);
        }
        [Fact]
        public void AddSmartSql_Action()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSmartSql((sp, smartsqlBuilder) =>
            {
                smartsqlBuilder.UseAlias("AddSmartSql_Action");
            });
            var serviceProvider = services.BuildServiceProvider();
            GetSmartSqlService(serviceProvider);
        }
        [Fact]
        public void AddRepositoryFromAssembly()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSmartSql("AddRepositoryFromAssembly")
            .AddRepositoryFromAssembly(o =>
            {
                o.AssemblyString = "SmartSql.Test";
                o.Filter = (type) =>
                {
                    return type.Namespace == "SmartSql.Test.Repositories";
                };
            });
            var serviceProvider = services.BuildServiceProvider();
            GetSmartSqlService(serviceProvider);
            IAllPrimitiveRepository allPrimitiveRepository = serviceProvider.GetRequiredService<IAllPrimitiveRepository>();
        }
        [Fact]
        public void AddRepositoryFromAssembly_Alias()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSmartSql("AddRepositoryFromAssembly_Alias")
            .AddRepositoryFromAssembly(o =>
            {
                o.SmartSqlAlias = "AddRepositoryFromAssembly_Alias";
                o.AssemblyString = "SmartSql.Test";
                o.Filter = (type) =>
                {
                    return type.Namespace == "SmartSql.Test.Repositories";
                };
            });
            var serviceProvider = services.BuildServiceProvider();
            GetSmartSqlService(serviceProvider);
            IAllPrimitiveRepository allPrimitiveRepository = serviceProvider.GetRequiredService<IAllPrimitiveRepository>();
        }
    }
}
