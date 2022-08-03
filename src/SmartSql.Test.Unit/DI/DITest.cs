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
        public void AddSmartSqlByFunc()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSmartSql(sp => new SmartSqlBuilder().UseAlias("AddSmartSqlByFunc")
                .UseDataSource(DbProvider.SQLSERVER, ConnectionString));
            var serviceProvider = services.BuildServiceProvider();
            GetSmartSqlService(serviceProvider);
        }

        [Fact]
        public void AddSmartSqlByAction()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSmartSql((sp, smartsqlBuilder) => { smartsqlBuilder.UseAlias("AddSmartSqlByAction"); });
            var serviceProvider = services.BuildServiceProvider();
            GetSmartSqlService(serviceProvider);
        }

        [Fact]
        public void AddRepositoryFromAssembly()
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
        public void AddRepositoryFromAssemblyUseAlias()
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
}