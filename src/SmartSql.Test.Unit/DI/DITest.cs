using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmartSql.DataSource;
using SmartSql.DbSession;
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
            services.AddSmartSql();
            var serviceProvider = services.BuildServiceProvider();
            GetSmartSqlService(serviceProvider);
            SmartSqlContainer.Instance.Dispose();
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
                return SmartSqlBuilder.AddDataSource(DbProvider.SQLSERVER, ConnectionString);
            });
            var serviceProvider = services.BuildServiceProvider();
            GetSmartSqlService(serviceProvider);
            SmartSqlContainer.Instance.Dispose();
        }
        [Fact]
        public void AddSmartSql_Action()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSmartSql((sp, smartsqlBuilder) =>
            {
                smartsqlBuilder.UseAlias("SmartSqlIsGood");
            });
            var serviceProvider = services.BuildServiceProvider();
            GetSmartSqlService(serviceProvider);
            SmartSqlContainer.Instance.Dispose();
        }
        [Fact]
        public void AddRepositoryFromAssembly()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSmartSql()
            .AddRepositoryFromAssembly(o =>
            {
                o.AssemblyString = "SmartSql.Test.Unit";
                o.Filter = (type) =>
                {
                    return type.Namespace == "SmartSql.Test.Unit.DyRepository";
                };
            });
            var serviceProvider = services.BuildServiceProvider();
            GetSmartSqlService(serviceProvider);
            IAllPrimitiveRepository allPrimitiveRepository = serviceProvider.GetRequiredService<IAllPrimitiveRepository>();
            SmartSqlContainer.Instance.Dispose();
        }
        [Fact]
        public void AddRepositoryFromAssembly_Alias()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSmartSql("AddRepositoryFromAssembly_Alias")
            .AddRepositoryFromAssembly(o =>
            {
                o.SmartSqlAlias = "AddRepositoryFromAssembly_Alias";
                o.AssemblyString = "SmartSql.Test.Unit";
                o.Filter = (type) =>
                {
                    return type.Namespace == "SmartSql.Test.Unit.DyRepository";
                };
            });
            var serviceProvider = services.BuildServiceProvider();
            GetSmartSqlService(serviceProvider);
            IAllPrimitiveRepository allPrimitiveRepository = serviceProvider.GetRequiredService<IAllPrimitiveRepository>();
            SmartSqlContainer.Instance.Dispose();
        }
    }
}
