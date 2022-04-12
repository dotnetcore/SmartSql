using SmartSql.Configuration.Tags;
using SmartSql.Reflection;
using SmartSql.Test.Entities;
using System;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using SmartSql.Data;
using Xunit;

namespace SmartSql.Test.Unit.DbSessions
{
    [Collection("GlobalSmartSql")]
    public class DbSessionTest
    {
        protected ISqlMapper SqlMapper { get; }

        public DbSessionTest(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
        }

        #region Insert_From_RealSql

        private const string INSERT_SQL = @"INSERT INTO T_AllPrimitive
              (`Boolean`
              ,`Char`
              ,`Int16`
              ,`Int32`
              ,`Int64`
              ,`Single`
              ,`Decimal`
              ,`DateTime`
              ,`String`
              ,`Guid`
              ,`TimeSpan`
              ,`NumericalEnum`
              ,`NullableBoolean`
              ,`NullableChar`
              ,`NullableInt16`
              ,`NullableInt32`
              ,`NullableInt64`
              ,`NullableSingle`
              ,`NullableDecimal`
              ,`NullableDateTime`
              ,`NullableGuid`
              ,`NullableTimeSpan`
              ,`NullableNumericalEnum`
              ,NullableString)
        VALUES
              (?Boolean
              ,?Char
              ,?Int16
              ,?Int32 
              ,?Int64
              ,?Single
              ,?Decimal
              ,?DateTime
              ,?String
              ,?Guid
              ,?TimeSpan
              ,?NumericalEnum
              ,?NullableBoolean
              ,?NullableChar
              ,?NullableInt16
              ,?NullableInt32
              ,?NullableInt64
              ,?NullableSingle
              ,?NullableDecimal
              ,?NullableDateTime
              ,?NullableGuid
              ,?NullableTimeSpan
              ,?NullableNumericalEnum
              ,?NullableString);
        Select Last_Insert_Id();";

        [Fact]
        public void Insert_From_RealSql()
        {
            var id = SqlMapper.ExecuteScalar<long>(new RequestContext
            {
                RealSql = INSERT_SQL,
                Request = new AllPrimitive
                {
                    DateTime = DateTime.Now,
                    String = "SmartSql",
                }
            });
            Assert.NotEqual(0, id);
        }

        #endregion

        [Fact]
        public void Insert()
        {
            var id = SqlMapper.ExecuteScalar<long>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "Insert",
                Request = new AllPrimitive
                {
                    DateTime = DateTime.Now,
                    String = "SmartSql",
                }
            });
        }

        [Fact]
        public void InsertByRequestTransaction()
        {
            var id = SqlMapper.ExecuteScalar<long>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "Insert",
                Transaction = IsolationLevel.Unspecified,
                Request = new AllPrimitive
                {
                    DateTime = DateTime.Now,
                    String = "SmartSql",
                }
            });
        }

        [Fact]
        public void InsertByStatementTransaction()
        {
            var id = SqlMapper.ExecuteScalar<long>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "InsertByStatementTransaction",
                Request = new AllPrimitive
                {
                    DateTime = DateTime.Now,
                    String = "SmartSql",
                }
            });
        }

        [Fact]
        public void InsertByIdGen()
        {
            var entity = new AllPrimitive
            {
                DateTime = DateTime.Now,
                String = "SmartSql",
            };
            var id = SqlMapper.ExecuteScalar<long>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "InsertByIdGen",
                Request = entity
            });
        }

        [Fact]
        public void InsertByIdGenAssignId()
        {
            var entity = new AllPrimitive
            {
                DateTime = DateTime.Now,
                String = "SmartSql",
            };
            var affected = SqlMapper.Execute(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "InsertByIdGenAssignId",
                Request = entity
            });
            Assert.True(entity.Int64 > 0);
        }

        [Fact]
        public async Task QueryAsync()
        {
            var list = await SqlMapper.QueryAsync<dynamic>(new RequestContext
            {
                RealSql = "SELECT T.* From T_AllPrimitive T limit 5"
            });

            Assert.NotNull(list);
        }

        [Fact]
        public async Task InsertAsync()
        {
            var id = await SqlMapper.ExecuteScalarAsync<long>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "Insert",
                Request = new AllPrimitive
                {
                    DateTime = DateTime.Now,
                    String = "SmartSql",
                }
            });
            Assert.NotEqual(0, id);
        }

        [Fact]
        public void InsertFromSqlParameters()
        {
            var insertParameters = RequestConvert.Instance.ToSqlParameters(new AllPrimitive
            {
                DateTime = DateTime.Now,
                String = "SmartSql",
            }, false);

            var id = SqlMapper.ExecuteScalar<long>(new RequestContext
            {
                RealSql = INSERT_SQL,
                Request = insertParameters
            });
            Assert.NotEqual(0, id);
        }

        [Fact]
        public void Update()
        {
            var id = SqlMapper.ExecuteScalar<int>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "Update",
                Request = new AllPrimitive
                {
                    DateTime = DateTime.Now,
                    String = "SmartSql",
                }
            });
        }

        [Fact]
        public void Delete()
        {
            var id = SqlMapper.ExecuteScalar<int>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "Delete",
                Request = new AllPrimitive
                {
                    DateTime = DateTime.Now,
                    String = "SmartSql",
                }
            });
        }

        [Fact]
        public void DeleteCheckIncludeRequired()
        {
            try
            {
                var id = SqlMapper.ExecuteScalar<int>(new RequestContext
                {
                    Scope = nameof(AllPrimitive),
                    SqlId = "DeleteCheckIncludeRequired",
                    Request = new { }
                });
            }
            catch (TagRequiredFailException ex)
            {
                Assert.True(true);
            }
        }

        [Fact]
        public void DeleteCheckIsNotEmptyRequired()
        {
            try
            {
                var id = SqlMapper.ExecuteScalar<int>(new RequestContext
                {
                    Scope = nameof(AllPrimitive),
                    SqlId = "DeleteCheckIsNotEmptyRequired",
                    Request = new { }
                });
            }
            catch (TagRequiredFailException ex)
            {
                Assert.True(true);
            }
        }

        [SkipGitHubCI]
        public void SP()
        {
            SqlParameterCollection dbParameterCollection = new SqlParameterCollection();
            dbParameterCollection.Add(new SqlParameter
            {
                Name = "Total",
                DbType = System.Data.DbType.Int32,
                Direction = System.Data.ParameterDirection.Output
            });
            RequestContext context = new RequestContext
            {
                CommandType = System.Data.CommandType.StoredProcedure,
                RealSql = "SP_Query",
                Request = dbParameterCollection
            };
            var list = SqlMapper.Query<AllPrimitive>(context);
            dbParameterCollection.TryGetParameterValue("Total", out int total);
        }

        [SkipGitHubCI]
        public void SP_SourceParameter()
        {
            SqlParameterCollection dbParameterCollection = new SqlParameterCollection();
            dbParameterCollection.Add(new SqlParameter("Total", null)
            {
                SourceParameter = new MySqlParameter("Total", DbType.Int32)
                {
                    Direction = ParameterDirection.Output
                }
            });
            RequestContext context = new RequestContext
            {
                CommandType = CommandType.StoredProcedure,
                RealSql = "SP_Query",
                Request = dbParameterCollection
            };
            var list = SqlMapper.Query<AllPrimitive>(context);
            Assert.NotNull(list);
            dbParameterCollection.TryGetParameterValue("Total", out int total);
        }
    }
}