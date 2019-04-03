using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Test.Repositories;
using Xunit;

namespace SmartSql.Test.Unit.DyRepository
{
    public class AllPrimitiveRepositoryTest : DyRepositoryTest
    {

        private IAllPrimitiveRepository _repository;
        public AllPrimitiveRepositoryTest()
        {
            var smartSqlBuilder = new SmartSqlBuilder().UseXmlConfig().Build();
            _repository = RepositoryFactory.CreateInstance(typeof(IAllPrimitiveRepository), smartSqlBuilder.SqlMapper) as IAllPrimitiveRepository;
        }
        [Fact]
        public void GetByPage_ValueTuple()
        {
            var result = _repository.GetByPage_ValueTuple();
            
            Assert.NotNull(result);
        }
    }
}
