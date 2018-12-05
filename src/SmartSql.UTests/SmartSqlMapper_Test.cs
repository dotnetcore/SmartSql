using SmartSql.Abstractions;
using SmartSql.UTests.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace SmartSql.UTests
{
    public class SmartSqlMapper_Test : TestBase, IDisposable
    {
        ISmartSqlMapper _sqlMapper;
        public SmartSqlMapper_Test()
        {
            _sqlMapper = MapperContainer.Instance.GetSqlMapper(new SmartSqlOptions
            {
                Alias = "SmartSqlMapper_Test",
                ConfigPath = Consts.DEFAULT_SMARTSQL_CONFIG_PATH
            });
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
        public void QuerySingle_N_HasVal()
        {
            var result = _sqlMapper.QuerySingle<DateTime?>(new RequestContext
            {
                Scope = Scope,
                RealSql = "Select GetDate();"
            });
        }
        [Fact]
        public void QuerySingle_N()
        {
            var result = _sqlMapper.QuerySingle<DateTime?>(new RequestContext
            {
                Scope = Scope,
                RealSql = "Select NUll;"
            });
        }
        [Fact]
        public void ExecuteScalar_N_HasVal()
        {
            var result = _sqlMapper.ExecuteScalar<DateTime?>(new RequestContext
            {
                Scope = Scope,
                RealSql = "Select GetDate();"
            });
        }
        [Fact]
        public void ExecuteScalar_N()
        {
            var result = _sqlMapper.ExecuteScalar<DateTime?>(new RequestContext
            {
                Scope = Scope,
                RealSql = "Select NUll;"
            });
        }
        [Fact]
        public void Query_ReadDb2()
        {
            var result = _sqlMapper.Query<T_Entity>(new RequestContext
            {
                Scope = Scope,
                SqlId = "Query_ReadDb2"
            });
        }
        [Fact]
        public void GetNested_Root()
        {
            var result = _sqlMapper.GetNested<QueryByPageResponse>(new RequestContext
            {
                Scope = Scope,
                SqlId = "GetNested_Root"
            });
        }
        [Fact]
        public void GetNested()
        {
            var result = _sqlMapper.GetNested<QueryByPageResponse>(new RequestContext
            {
                Scope = Scope,
                SqlId = "MQueryByPage"
            });
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
        public void InsertBatch_For()
        {
            var items = new List<T_Entity> {
                new T_Entity
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
                },new T_Entity
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
            };
            _sqlMapper.Execute(new RequestContext
            {
                Scope = Scope,
                SqlId = "InsertBatch",
                Request = new { Items = items }
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
                SqlId = "GetRecord",
                Request = new { FLongs = new long[] { 1, 2, 3 } }
            };
            var total = _sqlMapper.ExecuteScalar<long?>(context);
        }

        [Fact]
        public void ExecuteScalar_NULL()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "ExecuteScalar_NULL",
                Request = new { FLongs = new long[] { 1, 2, 3 } }
            };
            var total = _sqlMapper.ExecuteScalar<long?>(context);
        }

        [Fact]
        public void ExecuteScalar_Bool_True()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "IsExist"
            };
            var isExist = _sqlMapper.ExecuteScalar<bool>(context);
            Assert.True(isExist);
        }

        [Fact]
        public void ExecuteScalar_Bool_False()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "IsExist",
                Request = new { FString = Guid.NewGuid().ToString() }
            };
            var isExist = _sqlMapper.ExecuteScalar<bool>(context);
            Assert.False(isExist);
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
        public void QueryForPlaceholder()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "QueryForPlaceholder",
                Request = new { TableName = "T_Entity" }
            };
            var list = _sqlMapper.Query<T_Entity>(context);
        }
        [Fact]
        public void QueryCustomConstructorEntity()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "QueryCustomConstructorEntity"
            };
            var list = _sqlMapper.Query<T_CustomConstructorEntity>(context);
        }
        [Fact]
        public void QueryPrivateEntity()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "Query",
                Request = new
                {
                    Taken = 10
                }
            };
            var list = _sqlMapper.Query<T_PrivateEntity>(context);
        }

        [Fact]
        public void Query_NestedParams()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "Query_NestedParams",
                Request = new
                {
                    Root = new
                    {
                        Second = new
                        {
                            Id = 1
                        }
                    }
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
        public void StoredProcedure_From_XML()
        {
            DbParameterCollection dbParameterCollection = new DbParameterCollection();
            dbParameterCollection.Add(new DbParameter
            {
                Name = "Total",
                DbType = System.Data.DbType.Int32,
                Direction = System.Data.ParameterDirection.Output
            });
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "SP_QueryByPage",
                Request = dbParameterCollection
            };
            var list = _sqlMapper.Query<T_Entity>(context);
            var total = dbParameterCollection.GetValue<int>("Total");
        }

        [Fact]
        public void StoredProcedure_From_RealSql()
        {
            DbParameterCollection dbParameterCollection = new DbParameterCollection();
            dbParameterCollection.Add(new DbParameter
            {
                Name = "Total",
                DbType = System.Data.DbType.Int32,
                Direction = System.Data.ParameterDirection.Output
            });
            RequestContext context = new RequestContext
            {
                CommandType = System.Data.CommandType.StoredProcedure,
                RealSql = "SP_QueryByPage",
                Request = dbParameterCollection
            };
            var list = _sqlMapper.Query<T_Entity>(context);
            var total = dbParameterCollection.GetValue<int>("Total");
        }

        [Fact]
        public void FillMultiple()
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
            MultipleResult multipleResult = new MultipleResult();
            multipleResult.AddTypeMap<T_Entity>();
            multipleResult.AddTypeMap<T_Entity>();
            _sqlMapper.FillMultiple(context, multipleResult);
            var val1 = multipleResult.Get<T_Entity>(0);
            var val2 = multipleResult.Get<T_Entity>(1);
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


        [Fact]
        public void GetDbTable()
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
            var dataTable = _sqlMapper.GetDbTable(context);
        }
        [Fact]
        public void GetDbSet()
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
            var dataSet = _sqlMapper.GetDbSet(context);
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
        public async Task<long> ExecuteScalarAsync()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "GetRecord",
                Request = new { FStrings = new string[] { "SmartSql", "SmartCode" } }
            };

            var total = await _sqlMapper.ExecuteScalarAsync<long>(context);
            return total;
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
        public void QuerySingleM()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "GetEntity",
                Request = new { Id = 1000 }
            };
            var user = _sqlMapper.QuerySingle<T_Entity>(context);
            RequestContext context1 = new RequestContext
            {
                Scope = Scope,
                SqlId = "GetEntity",
                Request = new { Id = 1000 }
            };
            var user1 = _sqlMapper.QuerySingle<T_Entity>(context1);
            Assert.Equal(user.Id, user1.Id);
        }

        [Fact]
        public async Task QueryMultipleAsync()
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
            MultipleResult multipleResult = new MultipleResult();
            multipleResult.AddTypeMap<T_Entity>();
            multipleResult.AddTypeMap<T_Entity>();
            await _sqlMapper.FillMultipleAsync(context, multipleResult);
            var val1 = multipleResult.Get<T_Entity>(0);
            var val2 = multipleResult.Get<T_Entity>(1);
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
        public async Task GetDbTableAsync()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "MultiQuery"
            };
            var dataTable = await _sqlMapper.GetDbTableAsync(context);
        }

        [Fact]
        public async Task GetDbSetAsync()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "MultiQuery"
            };
            var dataSet = await _sqlMapper.GetDbSetAsync(context);
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
    public class QueryByPageResponse
    {
        public int Total { get; set; }
        public IEnumerable<T_Entity> List { get; set; }
    }
}
