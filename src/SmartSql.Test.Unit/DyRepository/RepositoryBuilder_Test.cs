using SmartSql.DataSource;
using SmartSql.DyRepository;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit.DyRepository
{
    public class RepositoryBuilder_Test : AbstractXmlConfigBuilderTest
    {
        IRepositoryBuilder _repositoryBuilder;
        IRepositoryFactory _repositoryFactory;
        public RepositoryBuilder_Test()
        {
            _repositoryBuilder = new EmitRepositoryBuilder(null, null, Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance);
            _repositoryFactory = new RepositoryFactory(_repositoryBuilder, Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance);
        }
        [Fact]
        public void Build()
        {
            var repositoryImplType = _repositoryBuilder.Build(typeof(IAllPrimitiveRepository), SmartSqlBuilder.SmartSqlConfig);
        }
        [Fact]
        public void CreateInstance()
        {
            var sqlMapper = SmartSqlBuilder.GetSqlMapper();
            var repository = _repositoryFactory.CreateInstance(typeof(IAllPrimitiveRepository), sqlMapper) as IAllPrimitiveRepository;

            var id = repository.Insert(new Entities.AllPrimitive
            {
                String = "",
                DateTime = DateTime.Now
            });
            var list = repository.Query(10);
        }


        [Fact]
        public void NoMapperRepository_GetGuidFromDb()
        {
            var sqlMapper = SmartSqlBuilder.GetSqlMapper();
            var repository = _repositoryFactory.CreateInstance(typeof(INoMapperRepository), sqlMapper) as INoMapperRepository;

            var guid = repository.GetGuidFromDb();
        }
        [Fact]
        public void NoMapperRepository_GetEntity()
        {
            var sqlMapper = SmartSqlBuilder.GetSqlMapper();
            var repository = _repositoryFactory.CreateInstance(typeof(INoMapperRepository), sqlMapper) as INoMapperRepository;

            var entity = repository.GetAllPrimitive();
        }
    }
}
