using SmartSql.DataSource;
using SmartSql.DyRepository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SmartSql.Test.Repositories;
using Xunit;
using Microsoft.Extensions.Logging;

namespace SmartSql.Test.Unit.DyRepository
{
    public class RepositoryBuilderTest
    {
        protected ISqlMapper SqlMapper { get; }

        IRepositoryBuilder _repositoryBuilder;
        IRepositoryFactory _repositoryFactory;

        public RepositoryBuilderTest()
        {
            var loggerFactory = new LoggerFactory(Enumerable.Empty<ILoggerProvider>(),
                new LoggerFilterOptions {MinLevel = LogLevel.Debug});
            var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs",
                "SmartSql-RepositoryBuilderTest.log");
            loggerFactory.AddFile(logPath, LogLevel.Trace);

            var smartSqlBuilder = new SmartSqlBuilder()
                .UseXmlConfig()
                .UseLoggerFactory(loggerFactory)
                .UseAlias(nameof(RepositoryBuilderTest) + Guid.NewGuid())
                .AddFilter<TestPrepareStatementFilter>()
                .Build();
            SqlMapper = smartSqlBuilder.SqlMapper;
            _repositoryBuilder = new EmitRepositoryBuilder(null, null,
                loggerFactory.CreateLogger<EmitRepositoryBuilder>());
            _repositoryFactory = new RepositoryFactory(_repositoryBuilder,
                loggerFactory.CreateLogger<RepositoryFactory>());
        }

        [Fact]
        public void Build()
        {
            var repositoryImplType =
                _repositoryBuilder.Build(typeof(IAllPrimitiveRepository), SqlMapper.SmartSqlConfig);
        }

        [Fact]
        public void CreateInstance()
        {
            var repository =
                _repositoryFactory.CreateInstance(typeof(IAllPrimitiveRepository),
                    SqlMapper) as IAllPrimitiveRepository;
            Assert.True(repository.IsDyRepository());
            var list = repository.Query(10);
            var id = repository.Insert(new Entities.AllPrimitive
            {
                String = "",
                DateTime = DateTime.Now
            });
        }

        [Fact]
        public void InsertByAnnotationTransaction()
        {
            var repository =
                _repositoryFactory.CreateInstance(typeof(IAllPrimitiveRepository),
                    SqlMapper) as IAllPrimitiveRepository;
            var id = repository.InsertByAnnotationTransaction(new Entities.AllPrimitive
            {
                String = "",
                DateTime = DateTime.Now
            });
        }

        [Fact]
        public void InsertByAnnotationAOPTransaction()
        {
            var repository =
                _repositoryFactory.CreateInstance(typeof(IAllPrimitiveRepository),
                    SqlMapper) as IAllPrimitiveRepository;
            var id = repository.InsertByAnnotationAOPTransaction(new Entities.AllPrimitive
            {
                String = "",
                DateTime = DateTime.Now
            });
        }
    }
}