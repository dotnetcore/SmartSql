using System;
using System.Collections.Generic;
using System.Data;
using StackExchange.Redis;
using Xunit;

namespace SmartSql.Test.Unit.DbSessions
{
    [Collection("GlobalSmartSql")]
    public class NestTest
    {
        protected ISqlMapper SqlMapper { get; }

        public NestTest(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
        }

        [Fact]
        public void QueryNestObject1()
        {
            
            var result = SqlMapper.ExecuteScalar<int>(new RequestContext
            {
                RealSql = "SELECT @User.Id",
                Request = new {User = new {Id = 1}}
            });

            Assert.Equal(1, result);
        }

        [Fact]
        public void QueryNestObject2()
        {
            var result = SqlMapper.ExecuteScalar<int>(new RequestContext
            {
                RealSql = "SELECT @User.Info.Id",
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
            });

            Assert.Equal(1, result);
        }

        [Fact]
        public void QueryNestArray()
        {
            var result = SqlMapper.ExecuteScalar<int>(new RequestContext
            {
                RealSql = "SELECT @Order.Items[0]",
                Request = new
                {
                    Order = new
                    {
                        Items = new int[] {1}
                    }
                }
            });

            Assert.Equal(1, result);
        }

        [Fact]
        public void QueryNestList()
        {
            var result = SqlMapper.ExecuteScalar<int>(new RequestContext
            {
                RealSql = "SELECT @Order.Items[0]",
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
            });

            Assert.Equal(1, result);
        }

        [Fact]
        public void QueryNestDic()
        {
            var result = SqlMapper.ExecuteScalar<int>(new RequestContext
            {
                Scope = nameof(NestTest),
                SqlId = "QueryNestDic",
                Request = new
                {
                    Order = new
                    {
                        Items = new Dictionary<string, int>
                        {
                            {"Id", 1}
                        }
                    }
                }
            });

            Assert.Equal(1, result);
        }

        [Fact]
        public void QueryNestArrayObject()
        {
            var result = SqlMapper.ExecuteScalar<String>(new RequestContext
            {
                Scope = nameof(NestTest),
                SqlId = "QueryNestArrayObject",
                Request = new
                {
                    Order = new
                    {
                        Items = new[]
                        {
                            new {Name = "SmartSql"}
                        }
                    }
                }
            });

            Assert.Equal("SmartSql", result);
        }

        [Fact]
        public void QueryNestArrayStrongObject()
        {
            var result = SqlMapper.ExecuteScalar<String>(new RequestContext
            {
                Scope = nameof(NestTest),
                SqlId = "QueryNestArrayObject",
                Request = new
                {
                    Order = new
                    {
                        Items = new[]
                        {
                            new OrderItem {Name = "SmartSql"}
                        }
                    }
                }
            });

            Assert.Equal("SmartSql", result);
        }


        [Fact]
        public void FilterNestObject1()
        {
            var result = SqlMapper.ExecuteScalar<int>(new RequestContext
            {
                Scope = nameof(NestTest),
                SqlId = "FilterNestObject1",
                Request = new {User = new {Id = 1}}
            });

            Assert.Equal(1, result);
        }

        [Fact]
        public void FilterNestObject2()
        {
            var result = SqlMapper.ExecuteScalar<int>(new RequestContext
            {
                Scope = nameof(NestTest),
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
            });

            Assert.Equal(1, result);
        }

        [Fact]
        public void FilterNestArray()
        {
            var result = SqlMapper.ExecuteScalar<int>(new RequestContext
            {
                Scope = nameof(NestTest),
                SqlId = "FilterNestArray",
                Request = new
                {
                    Order = new
                    {
                        Items = new int[] {1}
                    }
                }
            });

            Assert.Equal(1, result);
        }

        [Fact]
        public void FilterNestDic()
        {
            var result = SqlMapper.ExecuteScalar<int>(new RequestContext
            {
                Scope = nameof(NestTest),
                SqlId = "FilterNestDic",
                Request = new
                {
                    Order = new
                    {
                        Items = new Dictionary<string, int>
                        {
                            {"Id", 1}
                        }
                    }
                }
            });

            Assert.Equal(1, result);
        }

        [Fact]
        public void FilterNestArrayObject()
        {
            var result = SqlMapper.ExecuteScalar<String>(new RequestContext
            {
                Scope = nameof(NestTest),
                SqlId = "FilterNestArrayObject",
                Request = new
                {
                    Order = new
                    {
                        Items = new[]
                        {
                            new OrderItem {Name = "SmartSql"}
                        }
                    }
                }
            });

            Assert.Equal("SmartSql", result);
        }

        [Fact]
        public void FilterNestDicMul()
        {
            var result = SqlMapper.ExecuteScalar<String>(new RequestContext
            {
                Scope = nameof(NestTest),
                SqlId = "FilterNestDicMul",
                Request = new
                {
                    Fields = new Dictionary<String, String>
                    {
                        {"Id", "Id"},
                        {"Name", "Name"},
                        {"CreateTime", "CreateTime"},
                    }
                }
            });

            Assert.Equal("Id , Name , CreateTime", result.Trim());
        }

        public class OrderItem
        {
            public String Name { get; set; }
        }
    }
}