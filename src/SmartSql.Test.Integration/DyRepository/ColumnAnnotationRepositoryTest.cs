using System;
using SmartSql.DyRepository;
using SmartSql.Test.Repositories;
using Xunit;
using Microsoft.Extensions.Logging;
using SmartSql.Test.Entities;

namespace SmartSql.Test.Integration.DyRepository
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
            var id = _repository.Insert(new ColumnAnnotationEntity
            {
                Name = nameof(IColumnAnnotationRepository),
                Data = new ColumnAnnotationEntity.ExtendData
                {
                    Info = nameof(IColumnAnnotationRepository)
                }
            });
            return id;
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