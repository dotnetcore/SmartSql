using SmartSql.DataSource;
using SmartSql.DyRepository;
using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Test.Repositories;
using Xunit;

namespace SmartSql.Test.Unit.DyRepository
{
    public class RepositoryBuilder_Test : AbstractXmlConfigBuilderTest
    {
        protected ISqlMapper SqlMapper { get; }

        IRepositoryBuilder _repositoryBuilder;
        IRepositoryFactory _repositoryFactory;
        public RepositoryBuilder_Test()
        {
            SqlMapper = BuildSqlMapper(this.GetType().FullName);
            _repositoryBuilder = new EmitRepositoryBuilder(null, null, Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance);
            _repositoryFactory = new RepositoryFactory(_repositoryBuilder, Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance);
        }
        [Fact]
        public void Build()
        {
            var repositoryImplType = _repositoryBuilder.Build(typeof(IAllPrimitiveRepository), SqlMapper.SmartSqlConfig);
        }
        [Fact]
        public void CreateInstance()
        {

            var repository = _repositoryFactory.CreateInstance(typeof(IAllPrimitiveRepository), SqlMapper) as IAllPrimitiveRepository;
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
            var repository = _repositoryFactory.CreateInstance(typeof(IAllPrimitiveRepository), SqlMapper) as IAllPrimitiveRepository;
            var id = repository.InsertByAnnotationTransaction(new Entities.AllPrimitive
            {
                String = "",
                DateTime = DateTime.Now
            });
        }
        

        [Fact]
        public void NoMapperRepository_GetGuidFromDb()
        {
            var repository = _repositoryFactory.CreateInstance(typeof(INoMapperRepository), SqlMapper) as INoMapperRepository;
            var innerSqlMapper = repository.SqlMapper;
            var guid = repository.GetGuidFromDb();
        }
        [Fact]
        public void NoMapperRepository_GetEntity()
        {
            var repository = _repositoryFactory.CreateInstance(typeof(INoMapperRepository), SqlMapper) as INoMapperRepository;

            var entity = repository.GetAllPrimitive();
        }
    }
}
