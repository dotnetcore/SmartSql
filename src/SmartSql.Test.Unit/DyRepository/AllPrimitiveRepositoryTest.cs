using SmartSql.Test.Entities;
using System.Linq;
using SmartSql.Test.Repositories;
using Xunit;

namespace SmartSql.Test.Unit.DyRepository
{
    [Collection("GlobalSmartSql")]
    public class AllPrimitiveRepositoryTest
    {
        private readonly IAllPrimitiveRepository _repository;
        private ISqlMapper _mapper;
        public AllPrimitiveRepositoryTest(SmartSqlFixture smartSqlFixture)
        {
            _repository = smartSqlFixture.AllPrimitiveRepository;
            _mapper = smartSqlFixture.SqlMapper;
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
        public void GetNumericalEnums(int id, NumericalEnum11 numericalEnum)
        {
            var list = _mapper.Query<NumericalEnum11?>(new RequestContext
            {
                RealSql = "SELECT NumericalEnum FROM T_AllPrimitive WHERE Id = ?id",
                Request = new { id }
            });

            Assert.NotNull(list);
            Assert.True(list.All(t => t == numericalEnum));

            var result = _repository.GetNumericalEnums(id);

            Assert.NotNull(result);
            Assert.True(result.All(t => t == numericalEnum));
        }
    }
}
