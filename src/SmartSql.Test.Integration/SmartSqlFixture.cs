using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using SmartSql.DbSession;
using SmartSql.DyRepository;
using SmartSql.Middlewares.Filters;
using SmartSql.Test.Entities;
using SmartSql.Test.Repositories;
using Xunit;

namespace SmartSql.Test.Integration
{
    public class SmartSqlFixture : System.IDisposable
    {
        public const string GLOBAL_SMART_SQL = "GlobalSmartSql";

        public SmartSqlFixture()
        {
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
                RepositoryFactory.CreateInstance(typeof(IUsedCacheRepository), SqlMapper) as
                    IUsedCacheRepository;
            AllPrimitiveRepository =
                RepositoryFactory.CreateInstance(typeof(IAllPrimitiveRepository), SqlMapper) as
                    IAllPrimitiveRepository;
            UserRepository =
                RepositoryFactory.CreateInstance(typeof(IUserRepository), SqlMapper) as
                    IUserRepository;
            ColumnAnnotationRepository =
                RepositoryFactory.CreateInstance(typeof(IColumnAnnotationRepository), SqlMapper) as
                    IColumnAnnotationRepository;
            InitTestData();
        }

        protected void InitTestData()
        {
            AllPrimitiveRepository.Truncate();
            for (int i = 0; i < 10; i++)
            {
                AllPrimitiveRepository.Insert(new AllPrimitive()
                {
                    NumericalEnum = i % 2 == 0 ? NumericalEnum.One : NumericalEnum.Two
                });
            }
        }

        public SmartSqlBuilder SmartSqlBuilder { get; }
        public IDbSessionFactory DbSessionFactory { get; }
        public ISqlMapper SqlMapper { get; }
        public ILoggerFactory LoggerFactory { get; }
        public IRepositoryBuilder RepositoryBuilder { get; }
        public IRepositoryFactory RepositoryFactory { get; }

        #region Repository

        public IUsedCacheRepository UsedCacheRepository { get; }
        public IAllPrimitiveRepository AllPrimitiveRepository { get; }
        public IUserRepository UserRepository { get; }
        public IColumnAnnotationRepository ColumnAnnotationRepository { get; }

        #endregion

        public void Dispose()
        {
            SmartSqlBuilder.Dispose();
        }
    }

    [CollectionDefinition(SmartSqlFixture.GLOBAL_SMART_SQL)]
    public class SmartSqlCollection : ICollectionFixture<SmartSqlFixture>
    {
    }
}
