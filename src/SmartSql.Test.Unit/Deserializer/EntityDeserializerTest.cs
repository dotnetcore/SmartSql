using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SmartSql.Test.Unit.Deserializer
{
    [Collection("GlobalSmartSql")]
    public class EntityDeserializerTest
    {
        protected ISqlMapper SqlMapper { get; }

        public EntityDeserializerTest(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
        }

        [Fact]
        public void QuerySingle()
        {
            long id = Insert();
            var entity = SqlMapper.QuerySingle<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "GetById",
                Request = new { Id = id }
            });
            Assert.Equal(id, entity.Id);
        }

        private long Insert()
        {
            return SqlMapper.Insert<AllPrimitive, long>(new AllPrimitive
            {
                String = "Insert",
                DateTime = DateTime.Now
            });
        }

        [Fact]
        public void Query()
        {
            var list = SqlMapper.Query<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "Query",
                Request = new { Taken = 10000 }
            });
            Assert.NotNull(list);
        }

        [Fact]
        public async Task QuerySingleAsync()
        {
            long id = Insert();
            var entity = await SqlMapper.QuerySingleAsync<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "GetById",
                Request = new { Id = id }
            });
            Assert.Equal(id, entity.Id);
        }

        [Fact]
        public async Task QueryAsync()
        {
            var list = await SqlMapper.QueryAsync<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "Query",
                Request = new { Taken = 10000 }
            });
            Assert.NotNull(list);
        }
        // Unit test for nested property mapping functionality
        // 嵌套属性映射功能的单元测试
        [Fact]
        public void NestedPropertyMappingTest()
        {
            // Execute query with nested property mapping
            // 执行包含嵌套属性映射的查询
            var list = SqlMapper.Query<NestedEntity>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "QueryNestedPropertyResult", // SQL statement ID (SQL语句ID)
                Request = new { Taken = 10000 }      // Query parameters (查询参数)
            });

            // Verify result validity
            // 验证结果有效性
            Assert.NotNull(list);  // Result list should not be null (结果列表不应为空)

            // Validate nested properties existence
            // 验证嵌套属性存在性
            Assert.NotNull(list.First().NestedProp1);                      // Level 1 nesting (第一层嵌套)
            Assert.NotNull(list.First().NestedProp1.NestedProp2);          // Level 2 nesting (第二层嵌套)
            Assert.NotNull(list.First().NestedProp1.NestedProp2.NestedProp3); // Level 3 nesting (第三层嵌套)

            // Test Purpose:
            // Verifies ORM's ability to handle multi-level nested object mapping
            // 验证ORM处理多级嵌套对象映射的能力
            // Key Validations:
            // 1. Correct parsing of nested property paths (正确解析嵌套属性路径)
            // 2. Proper object initialization at each level (各级对象正确初始化)
            // 3. Maintains data integrity through mapping (通过映射保持数据完整性)
        }
    }
}