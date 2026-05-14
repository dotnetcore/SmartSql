using FluentAssertions;
using System.Linq;
using SmartSql.Test.Entities;
using SmartSql.Test.Repositories;
using Xunit;

namespace SmartSql.Test.Integration.DyRepository
{
    public class AllPrimitiveRepositoryTest : IntegrationTestBase
    {
        private readonly IAllPrimitiveRepository _repository;

        public AllPrimitiveRepositoryTest(SmartSqlFixture fixture) : base(fixture)
        {
            _repository = fixture.AllPrimitiveRepository;
        }

        [Fact]
        public void GetByPage_ValueTuple()
        {
            var result = _repository.GetByPage_ValueTuple();
            Assert.NotNull(result);
        }

        [Fact]
        public void QueryDictionary()
        {
            var result = _repository.QueryDictionary(10);
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData(1, NumericalEnum11.One)]
        [InlineData(2, NumericalEnum11.Two)]
        public void GetNumericalEnums(int value, NumericalEnum11 numericalEnum)
        {
            var list = SqlMapper.Query<NumericalEnum11?>(new RequestContext
            {
                RealSql = "SELECT NumericalEnum FROM T_AllPrimitive WHERE NumericalEnum = ?value",
                Request = new { value }
            });
            Assert.NotNull(list);
            Assert.True(list.All(t => t == numericalEnum));

            var result = _repository.GetNumericalEnums(value);
            Assert.NotNull(result);
            Assert.True(result.All(t => t == numericalEnum));
        }
    }
}
