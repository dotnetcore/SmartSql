using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Test.Repositories;
using Xunit;

namespace SmartSql.Test.Unit.DyRepository
{
    [Collection("GlobalSmartSql")]
    public class AllPrimitiveRepositoryTest : DyRepositoryTest
    {
        private IAllPrimitiveRepository _repository;
        public AllPrimitiveRepositoryTest(SmartSqlFixture smartSqlFixture)
        {
            _repository = RepositoryFactory.CreateInstance(typeof(IAllPrimitiveRepository), smartSqlFixture.SqlMapper) as IAllPrimitiveRepository;
        }
        [Fact]
        public void GetByPage_ValueTuple()
        {
            var result = _repository.GetByPage_ValueTuple();
            
            Assert.NotNull(result);
        }
    }
}
