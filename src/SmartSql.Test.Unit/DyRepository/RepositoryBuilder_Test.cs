using SmartSql.DataSource;
using SmartSql.DyRepository;
using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Test.Repositories;
using Xunit;
using Microsoft.Extensions.Logging;

namespace SmartSql.Test.Unit.DyRepository
{
    [Collection("GlobalSmartSql")]
    public class RepositoryBuilder_Test
    {
        protected ISqlMapper SqlMapper { get; }

        IRepositoryBuilder _repositoryBuilder;
        IRepositoryFactory _repositoryFactory;
        public RepositoryBuilder_Test(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
            _repositoryBuilder = new EmitRepositoryBuilder(null, null, smartSqlFixture.LoggerFactory.CreateLogger<EmitRepositoryBuilder>());
            _repositoryFactory = new RepositoryFactory(_repositoryBuilder, smartSqlFixture.LoggerFactory.CreateLogger<RepositoryFactory>());

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
        public void InsertByAnnotationAOPTransaction()
        {
            var repository = _repositoryFactory.CreateInstance(typeof(IAllPrimitiveRepository), SqlMapper) as IAllPrimitiveRepository;
            var id = repository.InsertByAnnotationAOPTransaction(new Entities.AllPrimitive
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
