using SmartSql.DyRepository;
using SmartSql.UTests.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;
using SmartSql.Abstractions;

namespace SmartSql.UTests.DyRepository
{
    public class DyRepository_Tests : TestBase
    {
        IEntityRepository _repository;
        ISmartSqlMapper _sqlMapper;
        public DyRepository_Tests()
        {
            string scope_template = "I{Scope}Repository";
            var builder = new RepositoryBuilder(scope_template, null, LoggerFactory.CreateLogger<RepositoryBuilder>());
            var factory = new RepositoryFactory(builder, LoggerFactory.CreateLogger<RepositoryFactory>());
            _sqlMapper = MapperContainer.Instance.GetSqlMapper(new SmartSqlOptions
            {
                Alias = "DyRepository_Tests",
                ConfigPath = Consts.DEFAULT_SMARTSQL_CONFIG_PATH
            });
            _repository = factory.CreateInstance<IEntityRepository>(_sqlMapper);
        }

        [Fact]
        public void IsExist()
        {
            var isExist = _repository.IsExist(null);
            Assert.True(isExist);
        }
        [Fact]
        public void IsExist_False()
        {
            var sqlMapper = _repository.SqlMapper;
            var isExist = _repository.IsExist(new { FString = Guid.NewGuid().ToString() });
            Assert.False(isExist);
        }
        [Fact]
        public void SP_QueryByPage_From_XML()
        {
            DbParameterCollection dbParameterCollection = new DbParameterCollection();
            dbParameterCollection.Add(new DbParameter
            {
                Name = "Total",
                DbType = System.Data.DbType.Int32,
                Direction = System.Data.ParameterDirection.Output
            });
            var list = _repository.SP_QueryByPage_From_XML(dbParameterCollection);
            var total = dbParameterCollection.GetValue<int>("Total");
        }
        [Fact]
        public void SP_QueryByPage_From_RealSql()
        {
            DbParameterCollection dbParameterCollection = new DbParameterCollection();
            dbParameterCollection.Add(new DbParameter
            {
                Name = "Total",
                DbType = System.Data.DbType.Int32,
                Direction = System.Data.ParameterDirection.Output
            });
            var list = _repository.SP_QueryByPage_From_RealSql(dbParameterCollection);
            var total = dbParameterCollection.GetValue<int>("Total");
        }
        [Fact]
        public void DeleteById()
        {
            var exc = _repository.DeleteById(1);
        }

        [Fact]
        public void GetById()
        {
            var enttiy = _repository.GetById(3);
        }
        [Fact]
        public void GetEntity()
        {
            var id = 10000;
            var enttiy = _repository.GetEntity(new { Id = id });
            var enttiy1 = _repository.GetEntity(new { Id = id });
            Assert.Equal<long>(enttiy.Id, enttiy1.Id);
        }
        [Fact]
        public void QueryBySql()
        {
            var list = _repository.QueryBySql(10);
        }

        [Fact]
        public void QueryMultiple_VT()
        {
            var vals = _repository.QueryMultiple_VT();
        }
        [Fact]
        public void QueryMultiple_VT_G()
        {
            var vals = _repository.QueryMultiple_VT<T_Entity>();
        }

        [Fact]
        public void QueryDataTable()
        {
            var dataTable = _repository.QueryDataTable();
        }
        [Fact]
        public void QueryDataSet()
        {
            var dataSet = _repository.QueryDataSet();
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
            var reqParams = new QueryByPageRequest { };
            var record = _repository.GetRecord(reqParams);
            var list = _repository.QueryByPage(reqParams);
        }
        [Fact]
        public void MQueryByPage()
        {
            var result = _repository.MQueryByPage();
        }
        [Fact]
        public void MQueryByPageG()
        {
            var result = _repository.MQueryByPage<QueryByPageResponse>();
        }
        [Fact]
        public async Task MQueryByPageAsync()
        {
            var result = await _repository.MQueryByPageAsync();
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
            var exc = _repository.DyUpdate(new
            {
                FLong = 2,
                FNullBool = true,
                LastUpdateTime = DateTime.Now
            });
        }

        [Fact]
        public void UpdateEntity()
        {
            var result = _repository.UpdateEntity(new T_Entity
            {
                Id = 1,
                CreationTime = DateTime.Now,
                FBool = true,
                FDecimal = 0,
                FLong = 0,
                FNullBool = false,
                FNullDecimal = 0,
                FString = "",
                LastUpdateTime = DateTime.Now,
                Status = EntityStatus.Ok
            });
        }

        [Fact]
        public async Task DeleteAsync()
        {
            await _repository.DeleteAsync(6);
        }
        [Fact]
        public async Task GetEntityAsync()
        {
            var enttiy = await _repository.GetEntityAsync(5);
        }
        [Fact]
        public async Task QueryBySqlAsync()
        {
            var list = await _repository.QueryBySqlAsync(10);
            var count = list.Count();
            Assert.NotNull(list);
        }
        [Fact]
        public async Task QueryDataTableAsync()
        {
            var dataTable = await _repository.QueryDataTableAsync();
        }
        [Fact]
        public async Task QueryDataSetAsync()
        {
            var dataSet = await _repository.QueryDataSetAsync();
        }
        [Fact]
        public async Task QueryAsync()
        {
            var list = await _repository.QueryAsync(10);

        }



        public void Dispose()
        {
            _sqlMapper.Dispose();
        }

        public class QueryByPageRequest
        {
            public int PageIndex { get; set; } = 1;
            public int PageSize { get; set; } = 10;

        }
    }
}
