using System;
using SmartSql.DyRepository;
using SmartSql.Test.Repositories;
using Xunit;
using Microsoft.Extensions.Logging;
using SmartSql.Test.Entities;

namespace SmartSql.Test.Unit.DyRepository
{
    [Collection("GlobalSmartSql")]
    public class ColumnAnnotationRepositoryTest
    {
        private IColumnAnnotationRepository _repository;

        public ColumnAnnotationRepositoryTest(SmartSqlFixture smartSqlFixture)
        {
            _repository = smartSqlFixture.ColumnAnnotationRepository;
        }

        [Fact]
        public void GetEntity()
        {
            var id = Insert();
            var entity = _repository.GetEntity(id);
            Assert.NotNull(entity);
            Assert.Equal(id, entity.Id);
        }

        [Fact]
        public int Insert()
        {
            var id = _repository.Insert(new ColumnAnnotationEntity
            {
                Name = nameof(IColumnAnnotationRepository),
                Data = new ColumnAnnotationEntity.ExtendData
                {
                    Info = nameof(IColumnAnnotationRepository)
                }
            });
            Assert.NotEqual(0, id);
            return id;
        }

        [Fact]
        public int InsertByParamAnnotations()
        {
            var id = _repository.Insert(nameof(InsertByParamAnnotations), new ColumnAnnotationEntity.ExtendData
            {
                Info = nameof(InsertByParamAnnotations)
            });
            Assert.NotEqual(0, id);
            return id;
        }
    }
}