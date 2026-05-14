using FluentAssertions;
using System;
using System.Data;
using System.Threading.Tasks;
using SmartSql.Data;
using SmartSql.Reflection.EntityProxy;
using SmartSql.Test.Entities;
using Xunit;


namespace SmartSql.Test.Integration.DbSession;

public class DbSessionTests : IntegrationTestBase
{
    public DbSessionTests(SmartSqlFixture fixture) : base(fixture) { }

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
    public void Should_ReturnId_When_InsertFromRealSql()
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
        id.Should().BeGreaterThan(0);
    }

    #endregion

    [Fact]
    public void Should_Insert_When_RequestIsValid()
    {
        SqlMapper.ExecuteScalar<long>(new RequestContext
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
    public void Should_Insert_When_RequestTransactionIsSpecified()
    {
        SqlMapper.ExecuteScalar<long>(new RequestContext
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
    public void Should_Insert_When_StatementTransactionIsConfigured()
    {
        SqlMapper.ExecuteScalar<long>(new RequestContext
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
    public void Should_Insert_When_IdGenIsUsed()
    {
        var entity = new AllPrimitive
        {
            DateTime = DateTime.Now,
            String = "SmartSql",
        };
        SqlMapper.ExecuteScalar<long>(new RequestContext
        {
            Scope = nameof(AllPrimitive),
            SqlId = "InsertByIdGen",
            Request = entity
        });
    }

    [Fact]
    public void Should_AssignId_When_InsertByIdGenAssignId()
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
        entity.Int64.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Should_ReturnList_When_QueryAsync()
    {
        var list = await SqlMapper.QueryAsync<dynamic>(new RequestContext
        {
            RealSql = "SELECT T.* From T_AllPrimitive T limit 5"
        });
        list.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_ReturnId_When_InsertAsync()
    {
        var id = await SqlMapper.ExecuteScalarAsync<long>(new RequestContext
        {
            Scope = nameof(AllPrimitive),
            SqlId = "InsertReturnId",
            Request = new AllPrimitive
            {
                DateTime = DateTime.Now,
                String = "SmartSql",
            }
        });
        id.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Should_ReturnId_When_InsertFromSqlParameters()
    {
        var insertParameters = SmartSql.Reflection.RequestConvert.Instance.ToSqlParameters(new AllPrimitive
        {
            DateTime = DateTime.Now,
            String = "SmartSql",
        }, false);

        var id = SqlMapper.ExecuteScalar<long>(new RequestContext
        {
            RealSql = INSERT_SQL,
            Request = insertParameters
        });
        id.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Should_Update_When_RequestIsValid()
    {
        SqlMapper.ExecuteScalar<int>(new RequestContext
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
    public void Should_Delete_When_ById()
    {
        SqlMapper.ExecuteScalar<int>(new RequestContext
        {
            Scope = nameof(AllPrimitive),
            SqlId = "DeleteById",
            Request = new AllPrimitive
            {
                DateTime = DateTime.Now,
                String = "SmartSql",
            }
        });
    }

    [Fact]
    public void Should_Throw_When_DeleteCheckIncludeRequiredFails()
    {
        Action act = () => SqlMapper.ExecuteScalar<int>(new RequestContext
        {
            Scope = nameof(AllPrimitive),
            SqlId = "DeleteCheckIncludeRequired",
            Request = new { }
        });
        act.Should().Throw<Configuration.Tags.TagRequiredFailException>();
    }

    [Fact]
    public void Should_Throw_When_DeleteCheckIsNotEmptyRequiredFails()
    {
        Action act = () => SqlMapper.ExecuteScalar<int>(new RequestContext
        {
            Scope = nameof(AllPrimitive),
            SqlId = "DeleteCheckIsNotEmptyRequired",
            Request = new { }
        });
        act.Should().Throw<Configuration.Tags.TagRequiredFailException>();
    }

    [EnvironmentFact(exclude: EnvironmentFactAttribute.GITHUB_ACTION)]
    public void Should_ExecuteStoredProcedure()
    {
        SqlParameterCollection dbParameterCollection = new SqlParameterCollection();
        dbParameterCollection.Add(new SqlParameter
        {
            Name = "Total",
            DbType = DbType.Int32,
            Direction = ParameterDirection.Output
        });
        RequestContext context = new RequestContext
        {
            CommandType = CommandType.StoredProcedure,
            RealSql = "SP_Query",
            Request = dbParameterCollection
        };
        var list = SqlMapper.Query<AllPrimitive>(context);
        dbParameterCollection.TryGetParameterValue("Total", out int total);
    }

    [EnvironmentFact(exclude: EnvironmentFactAttribute.GITHUB_ACTION)]
    public void Should_ExecuteStoredProcedure_When_SourceParameterProvided()
    {
        SqlParameterCollection dbParameterCollection = new SqlParameterCollection();
        dbParameterCollection.Add(new SqlParameter("Total", null)
        {
            SourceParameter = new MySql.Data.MySqlClient.MySqlParameter("Total", DbType.Int32)
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
        list.Should().NotBeNull();
        dbParameterCollection.TryGetParameterValue("Total", out int total);
    }
}
