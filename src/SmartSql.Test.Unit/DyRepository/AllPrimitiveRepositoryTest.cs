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
            _repository = smartSqlFixture.AllPrimitiveRepository;
        }
        // TODO
        [Fact(Skip = "TODO")]
        public void GetByPage_ValueTuple()
        {
            var result = _repository.GetByPage_ValueTuple();

            Assert.NotNull(result);
        }
        
        // TODO
        [Fact(Skip = "TODO")]
        public void QueryDictionary()
        {
            var result = _repository.QueryDictionary(10);

            Assert.NotNull(result);
        }
        
    }
}
