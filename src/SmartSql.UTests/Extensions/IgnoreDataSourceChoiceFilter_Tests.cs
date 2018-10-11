using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Abstractions;
using SmartSql.Extensions;
using Microsoft.Extensions.Logging;
using Xunit;
using SmartSql.UTests.Entity;

namespace SmartSql.UTests.Extensions
{
    public class IgnoreDataSourceChoiceFilter_Tests : TestBase, IDisposable
    {
        ISmartSqlMapper _sqlMapper;
        public IgnoreDataSourceChoiceFilter_Tests()
        {
            var logger = Logging.NoneLoggerFactory.Instance.CreateLogger<IgnoreDataSourceChoiceFilter>();
            _sqlMapper = MapperContainer.Instance.GetSqlMapper(new SmartSqlOptions
            {
                Alias = "IgnoreDataSourceChoiceFilter",
                ConfigPath = Consts.DEFAULT_SMARTSQL_CONFIG_PATH,
                DataSourceFilter = new IgnoreDataSourceChoiceFilter(logger)
            });
        }

        [Fact]
        public void Query_WriteDB_Elect()
        {
            var entity = _sqlMapper.QuerySingle<T_Entity>(new RequestContext
            {
                Scope = Scope,
                SqlId = "GetEntity",
                ReadDb = "WriteDB",
                Request = new { Id = 8 }
            });
        }
        [Fact]
        public void Query_ReadDb_Elect()
        {
            var entity = _sqlMapper.QuerySingle<T_Entity>(new RequestContext
            {
                Scope = Scope,
                SqlId = "GetEntity",
                ReadDb = "ReadDb-1",
                Request = new { Id = 8 }
            });
        }
        [Fact]
        public void Write_ReadDb_Elect()
        {
            _sqlMapper.Execute(new RequestContext
            {
                Scope = Scope,
                SqlId = "Insert",
                ReadDb = "ReadDb-2",
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
        public void Write_Write_Elect()
        {
            _sqlMapper.Execute(new RequestContext
            {
                Scope = Scope,
                SqlId = "Insert",
                ReadDb = "WriteDB",
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
        public void Query_Default_Elect()
        {
            var entity = _sqlMapper.QuerySingle<T_Entity>(new RequestContext
            {
                Scope = Scope,
                SqlId = "GetEntity",
                Request = new { Id = 8 }
            });
        }
        [Fact]
        public void Write_Default_Elect()
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
        public void Dispose()
        {
            _sqlMapper.Dispose();
        }
    }
}
