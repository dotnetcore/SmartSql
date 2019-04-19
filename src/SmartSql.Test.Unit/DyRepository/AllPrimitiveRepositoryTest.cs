using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Test.Repositories;
using Xunit;
using Microsoft.Extensions.Logging;
using SmartSql.DyRepository;

namespace SmartSql.Test.Unit.DyRepository
{
    [Collection("GlobalSmartSql")]
    public class AllPrimitiveRepositoryTest 
    {
        private IAllPrimitiveRepository _repository;
        public AllPrimitiveRepositoryTest(SmartSqlFixture smartSqlFixture)
        {
            var repositoryBuilder = new EmitRepositoryBuilder(null, null, smartSqlFixture.LoggerFactory.CreateLogger<EmitRepositoryBuilder>());
            var repositoryFactory = new RepositoryFactory(repositoryBuilder, smartSqlFixture.LoggerFactory.CreateLogger<RepositoryFactory>());
            _repository = repositoryFactory.CreateInstance(typeof(IAllPrimitiveRepository), smartSqlFixture.SqlMapper) as IAllPrimitiveRepository;
        }
        [Fact]
        public void GetByPage_ValueTuple()
        {
            var result = _repository.GetByPage_ValueTuple();

            Assert.NotNull(result);
        }
    }
}
