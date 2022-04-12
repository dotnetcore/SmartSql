using System;
using System.Collections.Generic;
using System.Data;
using SmartSql.Configuration;
using StackExchange.Redis;
using Xunit;

namespace SmartSql.Test.Unit.DbSessions
{
    [Collection("GlobalSmartSql")]
    public class NestTest
    {
        ISqlMapper SqlMapper { get; }

        public NestTest(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
        }

        [Fact]
        public void QueryNestObject1()
        {
            RequestContext requestCtx = new RequestContext
            {
                Scope = "NestTest",
                SqlId = "QueryNestObject1",
                Request = new { User = new { Id = 1 } }
            };
            var result = SqlMapper.ExecuteScalar<int>(requestCtx);
            Assert.Equal(1, result);
            Assert.Equal("Select ?User_Id", requestCtx.RealSql);
        }

        [Fact]
        public void QueryNestObject2()
        {
            RequestContext requestCtx = new RequestContext
            {
                Scope = "NestTest",
                SqlId = "QueryNestObject2",
                Request = new
                {
                    User = new
                    {
                        Info = new
                        {
                            Id = 1
                        }
                    }
                }
            };
            var result = SqlMapper.ExecuteScalar<int>(requestCtx);
            Assert.Equal(1, result);
            Assert.Equal("Select ?User_Info_Id", requestCtx.RealSql);
        }

        [Fact]
        public void QueryNestArray()
        {
            RequestContext requestCtx = new RequestContext
            {
                Scope = "NestTest",
                SqlId = "QueryNestArray",
                Request = new
                {
                    Order = new
                    {
                        Items = new[] { 1 }
                    }
                }
            };
            var result = SqlMapper.ExecuteScalar<int>(requestCtx);

            Assert.Equal(1, result);
            Assert.Equal("Select ?Order_Items_Idx_0", requestCtx.RealSql);
        }

        [Fact]
        public void QueryNestList()
        {
            RequestContext requestCtx = new RequestContext
            {
                Scope = "NestTest",
                SqlId = "QueryNestArray",
                Request = new
                {
                    Order = new
                    {
                        Items = new List<int>
                        {
                            1
                        }
                    }
                }
            };

            var result = SqlMapper.ExecuteScalar<int>(requestCtx);

            Assert.Equal(1, result);
            Assert.Equal("Select ?Order_Items_Idx_0", requestCtx.RealSql);
        }

        [Fact]
        public void QueryNestDic()
        {
            RequestContext requestCtx = new RequestContext
            {
                Scope = "NestTest",
                SqlId = "QueryNestDic",
                Request = new
                {
                    Order = new
                    {
                        Items = new Dictionary<string, int>
                        {
                            { "Id", 1 }
                        }
                    }
                }
            };

            var result = SqlMapper.ExecuteScalar<int>(requestCtx);

            Assert.Equal(1, result);
            Assert.Equal("Select ?Order_Items_Idx_Id", requestCtx.RealSql);
        }

        [Fact]
        public void QueryNestArrayObject()
        {
            RequestContext requestCtx = new RequestContext
            {
                Scope = "NestTest",
                SqlId = "QueryNestArrayObject",
                Request = new
                {
                    Order = new
                    {
                        Items = new[]
                        {
                            new { Name = "SmartSql" }
                        }
                    }
                }
            };

            var result = SqlMapper.ExecuteScalar<String>(requestCtx);

            Assert.Equal("SmartSql", result);
            Assert.Equal("Select ?Order_Items_Idx_0_Name", requestCtx.RealSql);
        }

        [Fact]
        public void QueryNestArrayStrongObject()
        {
            RequestContext requestCtx = new RequestContext
            {
                Scope = "NestTest",
                SqlId = "QueryNestArrayObject",
                Request = new
                {
                    Order = new
                    {
                        Items = new[]
                        {
                            new OrderItem { Name = "SmartSql" }
                        }
                    }
                }
            };

            var result = SqlMapper.ExecuteScalar<String>(requestCtx);

            Assert.Equal("SmartSql", result);
            Assert.Equal("Select ?Order_Items_Idx_0_Name", requestCtx.RealSql);
        }


        [Fact]
        public void FilterNestObject1()
        {
            RequestContext requestCtx = new RequestContext
            {
                Scope = "NestTest",
                SqlId = "FilterNestObject1",
                Request = new { User = new { Id = 1 } }
            };

            var result = SqlMapper.ExecuteScalar<int>(requestCtx);

            Assert.Equal(1, result);
            Assert.Equal("Select ?User_Id", requestCtx.RealSql);
        }

        [Fact]
        public void FilterNestObject2()
        {
            RequestContext requestCtx = new RequestContext
            {
                Scope = "NestTest",
                SqlId = "FilterNestObject2",
                Request = new
                {
                    User = new
                    {
                        Info = new
                        {
                            Id = 1
                        }
                    }
                }
            };

            var result = SqlMapper.ExecuteScalar<int>(requestCtx);

            Assert.Equal(1, result);
            Assert.Equal("Select ?User_Info_Id", requestCtx.RealSql);
        }

        [Fact]
        public void FilterNestArray()
        {
            RequestContext requestCtx = new RequestContext
            {
                Scope = nameof(NestTest),
                SqlId = "FilterNestArray",
                Request = new
                {
                    Order = new
                    {
                        Items = new[] { 1 }
                    }
                }
            };

            var result = SqlMapper.ExecuteScalar<int>(requestCtx);

            Assert.Equal(1, result);
            Assert.Equal("Select ?Order_Items_Idx_0", requestCtx.RealSql);
        }

        [Fact]
        public void FilterNestDic()
        {
            RequestContext requestCtx = new RequestContext
            {
                Scope = nameof(NestTest),
                SqlId = "FilterNestDic",
                Request = new
                {
                    Order = new
                    {
                        Items = new Dictionary<string, int>
                        {
                            { "Id", 1 }
                        }
                    }
                }
            };

            var result = SqlMapper.ExecuteScalar<int>(requestCtx);

            Assert.Equal(1, result);
            Assert.Equal("Select ?Order_Items_Idx_Id", requestCtx.RealSql);
        }

        [Fact]
        public void FilterNestArrayObject()
        {
            RequestContext requestCtx = new RequestContext
            {
                Scope = nameof(NestTest),
                SqlId = "FilterNestArrayObject",
                Request = new
                {
                    Order = new
                    {
                        Items = new[]
                        {
                            new OrderItem { Name = "SmartSql" }
                        }
                    }
                }
            };

            var result = SqlMapper.ExecuteScalar<String>(requestCtx);

            Assert.Equal("SmartSql", result);
            Assert.Equal("Select ?Order_Items_Idx_0_Name", requestCtx.RealSql);

        }

        
        [Fact]
        public void FilterNestDicMul()
        {
            RequestContext requestCtx = new RequestContext
            {
                Scope = nameof(NestTest),
                SqlId = "FilterNestDicMul",
                Request = new
                {
                    Fields = new Dictionary<String, String>
                    {
                        { "Id", "Id" },
                        { "Name", "Name" },
                        { "CreateTime", "CreateTime" },
                    }
                }
            };

            var result = SqlMapper.ExecuteScalar<String>(requestCtx);

            Assert.Equal("Id , Name , CreateTime", result.Trim());
            Assert.Equal(@"Select'
                Id , Name , CreateTime
            '", requestCtx.RealSql.Trim());
        }
        
        public class OrderItem
        {
            public String Name { get; set; }
        }
    }
}