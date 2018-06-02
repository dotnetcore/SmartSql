using SmartSql.DyRepository;
using SmartSql.UTests.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.UTests.DyRepository
{
    public class DyRepository_Tests : IDisposable
    {
        IEntityRepository _repository;
        public DyRepository_Tests()
        {
            string scope_template = "I{Scope}Repository";
            var builder = new RepositoryBuilder(scope_template);
            var factory = new RepositoryFactory(builder);
            var sqlMapper = MapperContainer.Instance.GetSqlMapper();
            _repository = factory.CreateInstance<IEntityRepository>(sqlMapper);
        }

        [Fact]
        public void Delete()
        {
            var exc = _repository.Delete(1);
        }
        [Fact]
        public void GetEntity()
        {
            var enttiy = _repository.GetEntity(3);

        }
        [Fact]
        public void Insert()
        {
            var id = _repository.Insert(new T_Entity
            {
                CreationTime = DateTime.Now,
                FString = "SmartSql-" + this.GetHashCode(),
                FBool = true,
                FDecimal = 1,
                FLong = 1,
                FNullBool = null,
                FNullDecimal = null,
                LastUpdateTime = null,
                NullStatus = null,
                Status = EntityStatus.Ok
            });
        }
        [Fact]
        public void Query()
        {
            var list = _repository.Query(new
            {
                Taken = 10
            });

        }
        [Fact]
        public void QueryByPage()
        {
            var list = _repository.QueryByPage(new
            {
                PageIndex = 1,
                PageSize = 10
            });
        }
        [Fact]
        public void QueryId()
        {
            var list = _repository.QueryId();
        }
        [Fact]
        public void QueryStatus()
        {
            var list = _repository.QueryStatus();
        }
        [Fact]
        public void QueryNullStatus()
        {
            var list = _repository.QueryNullStatus();
        }

        [Fact]
        public void Update()
        {
            var exc = _repository.Update(new
            {
                FLong = 2,
                FNullBool = true,
                LastUpdateTime = DateTime.Now
            });
        }

        public void Dispose()
        {
            MapperContainer.Instance.Dispose();
        }
    }
}
