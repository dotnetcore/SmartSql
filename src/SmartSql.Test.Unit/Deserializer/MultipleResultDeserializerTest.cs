using SmartSql.Test.DTO;
using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SmartSql.Test.Unit.Deserializer
{
    [Collection("GlobalSmartSql")]
    public class MultipleResultDeserializerTest 
    {
        protected ISqlMapper SqlMapper { get; }

        public MultipleResultDeserializerTest(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
        }

        [Fact]
        public void GetByPage()
        {
            var result = SqlMapper.QuerySingle<GetByPageResponse<AllPrimitive>>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "GetByPage",
                Request = new { PageSize = 10, PageIndex = 1 }
            });
            Assert.NotNull(result);
        }
        [Fact]
        public async Task GetByPageAsync()
        {
            var result = await SqlMapper.QuerySingleAsync<GetByPageResponse<AllPrimitive>>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "GetByPage",
                Request = new { PageSize = 10, PageIndex = 1 }
            });
            Assert.NotNull(result);
        }
        
        [Fact]
        public void GetMultiRoot()
        {
            var result = SqlMapper.QuerySingle<PagedList>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "GetMultiRoot",
                Request = new { PageSize = 10, PageIndex = 1 }
            });
            Assert.NotNull(result);
            Assert.NotNull(result.List);
        }
        
        public class PagedList
        {
            public long Total { get; set; }
            public IList<AllPrimitive> List { get; set; }
        }
    }
}
