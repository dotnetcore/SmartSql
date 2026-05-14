using System;
using System.Data;
using System.Threading.Tasks;
using FluentAssertions;
using SmartSql.Test.Entities;
using SmartSql.Test.Integration.Fixtures;
using Xunit;

namespace SmartSql.Test.Integration.Base;

public abstract class DbSessionTestBase : IntegrationTestBase
{
    protected DbSessionTestBase(IDbTestFixture fixture) : base(fixture) { }

    private string GetInsertSql()
    {
        var columns = "Boolean,`Char`,Int16,Int32,Int64,Single,`Decimal`,DateTime,String,Guid,TimeSpan,NumericalEnum," +
                      "NullableBoolean,NullableChar,NullableInt16,NullableInt32,NullableInt64,NullableSingle,NullableDecimal,NullableDateTime,NullableGuid,NullableTimeSpan,NullableNumericalEnum,NullableString";
        var values = "$Boolean,$Char,$Int16,$Int32,$Int64,$Single,$Decimal,$DateTime,$String,$Guid,$TimeSpan,$NumericalEnum," +
                     "$NullableBoolean,$NullableChar,$NullableInt16,$NullableInt32,$NullableInt64,$NullableSingle,$NullableDecimal,$NullableDateTime,$NullableGuid,$NullableTimeSpan,$NullableNumericalEnum,$NullableString";

        return DbProvider switch
        {
            "PostgreSql" => @$"INSERT INTO ""T_AllPrimitive"" ({columns.Replace("`", "\"")}) VALUES ({values}); RETURNING ""Id""",
            "SqlServer" => $"INSERT INTO T_AllPrimitive ({columns.Replace("`", "[").Replace("]", "]").Replace("[Char]", "[Char]").Replace("[Decimal]", "[Decimal]")}) VALUES ({values}); SELECT SCOPE_IDENTITY()",
            "SQLite" => @$"INSERT INTO T_AllPrimitive ({columns.Replace("`", "\"")}) VALUES ({values}); SELECT last_insert_rowid()",
            _ => $"INSERT INTO T_AllPrimitive ({columns}) VALUES ({values}); Select Last_Insert_Id()"
        };
    }

    [Fact]
    public void Should_ReturnId_When_InsertFromRealSql()
    {
        var id = SqlMapper.ExecuteScalar<long>(new RequestContext
        {
            RealSql = GetInsertSql(),
            Request = new AllPrimitive { DateTime = DateTime.Now, String = "SmartSql" }
        });
        id.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Should_Insert_When_RequestIsValid()
    {
        SqlMapper.ExecuteScalar<long>(new RequestContext
        {
            Scope = nameof(AllPrimitive),
            SqlId = "Insert",
            Request = new AllPrimitive { DateTime = DateTime.Now, String = "SmartSql" }
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
            Request = new AllPrimitive { DateTime = DateTime.Now, String = "SmartSql" }
        });
    }

    [Fact]
    public void Should_Insert_When_StatementTransactionIsConfigured()
    {
        SqlMapper.ExecuteScalar<long>(new RequestContext
        {
            Scope = nameof(AllPrimitive),
            SqlId = "InsertByStatementTransaction",
            Request = new AllPrimitive { DateTime = DateTime.Now, String = "SmartSql" }
        });
    }

    [Fact]
    public void Should_Insert_When_IdGenIsUsed()
    {
        var entity = new AllPrimitive { DateTime = DateTime.Now, String = "SmartSql" };
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
        var entity = new AllPrimitive { DateTime = DateTime.Now, String = "SmartSql" };
        SqlMapper.Execute(new RequestContext
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
            RealSql = SelectTopAllPrimitive(5)
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
            Request = new AllPrimitive { DateTime = DateTime.Now, String = "SmartSql" }
        });
        id.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Should_ReturnId_When_InsertFromSqlParameters()
    {
        var insertParameters = SmartSql.Reflection.RequestConvert.Instance.ToSqlParameters(
            new AllPrimitive { DateTime = DateTime.Now, String = "SmartSql" }, false);
        var id = SqlMapper.ExecuteScalar<long>(new RequestContext
        {
            RealSql = GetInsertSql(),
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
            Request = new AllPrimitive { DateTime = DateTime.Now, String = "SmartSql" }
        });
    }

    [Fact]
    public void Should_Delete_When_ById()
    {
        SqlMapper.ExecuteScalar<int>(new RequestContext
        {
            Scope = nameof(AllPrimitive),
            SqlId = "DeleteById",
            Request = new AllPrimitive { DateTime = DateTime.Now, String = "SmartSql" }
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
}
