using SmartSql.DyRepository;
using SmartSql.Test.Entities;
using SmartSql.Test.Repositories;
using Xunit;

namespace SmartSql.Test.Integration.DyRepository
{
    public class ColumnAnnotationRepositoryTest : IntegrationTestBase
    {
        private readonly IColumnAnnotationRepository _repository;

        public ColumnAnnotationRepositoryTest(SmartSqlFixture fixture) : base(fixture)
        {
            _repository = fixture.ColumnAnnotationRepository;
        }

        [Fact]
        public void GetEntity()
        {
            var id = DoInsert();
            var entity = _repository.GetEntity(id);
            Assert.NotNull(entity);
            Assert.Equal(id, entity.Id);
        }

        [Fact]
        public void Insert()
        {
            var id = DoInsert();
            Assert.NotEqual(0, id);
        }

        private int DoInsert()
        {
            return _repository.Insert(new ColumnAnnotationEntity
            {
                Name = nameof(IColumnAnnotationRepository),
                Data = new ColumnAnnotationEntity.ExtendData
                {
                    Info = nameof(IColumnAnnotationRepository)
                }
            });
        }

        [Fact]
        public void InsertByParamAnnotations()
        {
            var id = _repository.Insert(nameof(InsertByParamAnnotations), new ColumnAnnotationEntity.ExtendData
            {
                Info = nameof(InsertByParamAnnotations)
            });
            Assert.NotEqual(0, id);
        }
    }
}
