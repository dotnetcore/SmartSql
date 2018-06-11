using SmartSql.Abstractions;
using SmartSql.UTests.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SmartSql.UTests
{
    public class SmartSqlMapper_Test : TestBase, IDisposable
    {
        ISmartSqlMapper _sqlMapper;
        public SmartSqlMapper_Test()
        {
            _sqlMapper = MapperContainer.Instance.GetSqlMapper();
        }

        public void Dispose()
        {
            _sqlMapper.Dispose();
        }
        [Fact]
        public void BeginSession()
        {
            RequestContext context = new RequestContext { };
            _sqlMapper.BeginSession(context);
            _sqlMapper.EndSession();
        }
        [Fact]
        public void BeginTransaction()
        {
            try
            {
                _sqlMapper.BeginTransaction();
                var entity = _sqlMapper.QuerySingle<T_Entity>(new RequestContext
                {
                    Scope = Scope,
                    SqlId = "GetEntity",
                    Request = new { Id = 8 }
                });
                Insert();
                Insert();
                Insert();
                _sqlMapper.CommitTransaction();
            }
            catch (Exception ex)
            {
                _sqlMapper.RollbackTransaction();
                throw ex;
            }
        }
        [Fact]
        public void Insert()
        {
            _sqlMapper.Execute(new RequestContext
            {
                Scope = Scope,
                SqlId = "Insert",
                Request = new T_Entity
                {
                    CreationTime = DateTime.Now,
                    FBool = true,
                    FDecimal = 1,
                    FLong = 1,
                    FNullBool = false,
                    FString = Guid.NewGuid().ToString("N"),
                    FNullDecimal = 1.1M,
                    LastUpdateTime = DateTime.Now,
                    Status = EntityStatus.Ok
                }
            });
        }
        [Fact]
        public void Execute()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "Delete",
                Request = new { Id = 3 }
            };
            _sqlMapper.Execute(context);
        }
        [Fact]
        public void ExecuteScalar()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "GetRecord"
            };
            var total = _sqlMapper.ExecuteScalar<long>(context);
        }

        [Fact]
        public void QueryBySql()
        {
            RequestContext context = new RequestContext
            {
                RealSql = "Select Top(@Taken) T.* From T_Entity T With(NoLock);",
                Request = new
                {
                    Taken = 10
                }
            };
            var list = _sqlMapper.Query<T_Entity>(context);
        }

        [Fact]
        public void Query()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "Query",
                Request = new
                {
                    Taken = 10,
                    Ids = new long[] { 1, 2, 4 }
                }
            };
            var list = _sqlMapper.Query<T_Entity>(context);
        }
        [Fact]
        public void QuerySingle()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "GetEntity",
                Request = new { Id = 2 }
            };
            var entity = _sqlMapper.QuerySingle<T_Entity>(context);
        }
        [Fact]
        public void GetDataTable()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "MultiQuery",
                Request = new
                {
                    Taken = 10
                }
            };
            var dataTable = _sqlMapper.GetDataTable(context);
        }
        [Fact]
        public void GetDataSet()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "MultiQuery",
                Request = new
                {
                    Taken = 10
                }
            };
            var dataSet = _sqlMapper.GetDataSet(context);
        }

        #region Async
        [Fact]
        public async Task ExecuteAsync()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "Delete",
                Request = new { Id = 3 }
            };
            int exeNum = await _sqlMapper.ExecuteAsync(context);
        }
        [Fact]
        public async Task ExecuteScalarAsync()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "GetRecord"
            };
            var total = await _sqlMapper.ExecuteScalarAsync<long>(context);
        }
        [Fact]
        public async Task QueryAsync()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "Query",
                Request = new { Taken = 10 }
            };
            var users = await _sqlMapper.QueryAsync<T_Entity>(context);
        }

        [Fact]
        public async Task QuerySingleAsync()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "GetEntity",
                Request = new { Id = 2 }
            };
            var user = await _sqlMapper.QuerySingleAsync<T_Entity>(context);
        }
        [Fact]
        public async Task GetDataTableAsync()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "MultiQuery"
            };
            var dataTable = await _sqlMapper.GetDataTableAsync(context);
        }

        [Fact]
        public async Task GetDataSetAsync()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "MultiQuery"
            };
            var dataSet = await _sqlMapper.GetDataSetAsync(context);
        }
        [Fact]
        public async Task TransactionAsync()
        {
            try
            {
                _sqlMapper.BeginTransaction();
                var entity = await _sqlMapper.QuerySingleAsync<T_Entity>(new RequestContext
                {
                    Scope = Scope,
                    SqlId = "GetEntity",
                    Request = new { Id = 8 }
                });
                await InsertAsync();
                await InsertAsync();
                await InsertAsync();
                _sqlMapper.CommitTransaction();
            }
            catch (Exception ex)
            {
                _sqlMapper.RollbackTransaction();
                throw ex;
            }
        }

        [Fact]
        public async Task InsertAsync()
        {
            await _sqlMapper.ExecuteAsync(new RequestContext
            {
                Scope = Scope,
                SqlId = "Insert",
                Request = new T_Entity
                {
                    CreationTime = DateTime.Now,
                    FBool = true,
                    FDecimal = 1,
                    FLong = 1,
                    FNullBool = false,
                    FString = Guid.NewGuid().ToString("N"),
                    FNullDecimal = 1.1M,
                    LastUpdateTime = DateTime.Now,
                    Status = EntityStatus.Ok
                }
            });
        }
        #endregion
    }
}
