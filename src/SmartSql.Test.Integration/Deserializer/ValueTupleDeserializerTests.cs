using FluentAssertions;
using System;
using System.Collections.Generic;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Integration.Deserializer;

public class ValueTupleDeserializerTests : IntegrationTestBase
{
    public ValueTupleDeserializerTests(SmartSqlFixture fixture) : base(fixture) { }

    [Fact]
    public void Should_ReturnValueTuple_When_GetByPage()
    {
        var result = SqlMapper.QuerySingle<ValueTuple<IEnumerable<AllPrimitive>, int>>(new RequestContext
        {
            Scope = nameof(AllPrimitive),
            SqlId = "GetByPage_ValueTuple",
            Request = new { PageSize = 10, Offset = 0 }
        });
        result.Should().NotBeNull();
    }
}
