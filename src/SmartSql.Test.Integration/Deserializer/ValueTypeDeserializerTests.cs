using FluentAssertions;
using System;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Integration.Deserializer;

public class ValueTypeDeserializerTests : IntegrationTestBase
{
    public ValueTypeDeserializerTests(SmartSqlFixture fixture) : base(fixture) { }

    private const int ONE = 1;
    private const string SELECT_ONE = "Select 1;";

    #region Int

    [Fact]
    public void Should_ReturnInt_When_QueryingInt()
    {
        var result = SqlMapper.ExecuteScalar<int>(new RequestContext { RealSql = SELECT_ONE });
        result.Should().Be(ONE);
    }

    [Fact]
    public void Should_ReturnNullableInt_When_QueryingNullableInt()
    {
        var result = SqlMapper.ExecuteScalar<int?>(new RequestContext { RealSql = SELECT_ONE });
        result.Should().Be(ONE);
    }

    #endregion

    #region Enum

    [Fact]
    public void Should_ReturnEnum_When_QueryingEnum()
    {
        var result = SqlMapper.ExecuteScalar<NumericalEnum>(new RequestContext { RealSql = SELECT_ONE });
        result.Should().Be(NumericalEnum.One);
    }

    [Fact]
    public void Should_ReturnNullableEnum_When_QueryingNullableEnum()
    {
        var result = SqlMapper.ExecuteScalar<NumericalEnum?>(new RequestContext { RealSql = SELECT_ONE });
        result.Should().Be(NumericalEnum.One);
    }

    #endregion

    #region DateTime

    private const string DATE_TIME = "2019-08-08 08:08";
    private readonly static string SELECT_DATE_TIME = $"Select Convert('{DATE_TIME}',datetime)";
    private readonly static DateTime TEST_DATE_TIME = DateTime.Parse(DATE_TIME);

    [Fact]
    public void Should_ReturnDateTime_When_QueryingDateTime()
    {
        var result = SqlMapper.ExecuteScalar<DateTime>(new RequestContext { RealSql = SELECT_DATE_TIME });
        result.Should().Be(TEST_DATE_TIME);
    }

    [Fact]
    public void Should_ReturnNullableDateTime_When_QueryingNullableDateTime()
    {
        var result = SqlMapper.ExecuteScalar<DateTime?>(new RequestContext { RealSql = SELECT_DATE_TIME });
        result.Should().Be(TEST_DATE_TIME);
    }

    #endregion

    #region Timespan

    private readonly static TimeSpan TEST_TIMESPAN = TimeSpan.Parse("08:08:08");
    private readonly static string SELECT_TIMESPAN = $"Select CONVERT('{TEST_TIMESPAN:g}',time)";

    [Fact]
    public void Should_ReturnTimeSpan_When_QueryingTimeSpan()
    {
        var result = SqlMapper.ExecuteScalar<TimeSpan>(new RequestContext { RealSql = SELECT_TIMESPAN });
        result.Should().Be(TEST_TIMESPAN);
    }

    [Fact]
    public void Should_ReturnNullableTimeSpan_When_QueryingNullableTimeSpan()
    {
        var result = SqlMapper.ExecuteScalar<TimeSpan?>(new RequestContext { RealSql = SELECT_TIMESPAN });
        result.Should().Be(TEST_TIMESPAN);
    }

    #endregion

    #region Guid

    private const string GUID = "96061D08-C029-4A36-AB40-FDBFA546EC82";
    private readonly static string SELECT_GUID = $"Select '{GUID}'";
    private readonly static Guid TEST_GUID = new Guid(GUID);

    [Fact]
    public void Should_ReturnGuid_When_QueryingGuid()
    {
        var result = SqlMapper.ExecuteScalar<Guid>(new RequestContext { RealSql = SELECT_GUID });
        result.Should().Be(TEST_GUID);
    }

    [Fact]
    public void Should_ReturnNullableGuid_When_QueryingNullableGuid()
    {
        var result = SqlMapper.ExecuteScalar<Guid?>(new RequestContext { RealSql = SELECT_GUID });
        result.Should().Be(TEST_GUID);
    }

    #endregion

    #region String

    private const string STRING = "SmartSql";
    private readonly static string SELECT_STRING = $"Select '{STRING}';";

    [Fact]
    public void Should_ReturnString_When_QueryingString()
    {
        var result = SqlMapper.ExecuteScalar<string>(new RequestContext { RealSql = SELECT_STRING });
        result.Should().Be(STRING);
    }

    #endregion

    #region Null

    private readonly static string SELECT_NULL = "Select Null;";

    [Fact]
    public void Should_ReturnDefaultInt_When_QueryingNullAsInt()
    {
        var result = SqlMapper.ExecuteScalar<int>(new RequestContext { RealSql = SELECT_NULL });
        result.Should().Be(default(int));
    }

    [Fact]
    public void Should_ReturnNull_When_QueryingNullAsNullableInt()
    {
        var result = SqlMapper.ExecuteScalar<int?>(new RequestContext { RealSql = SELECT_NULL });
        result.Should().BeNull();
    }

    [Fact]
    public void Should_ReturnDefaultGuid_When_QueryingNullAsGuid()
    {
        var result = SqlMapper.ExecuteScalar<Guid>(new RequestContext { RealSql = SELECT_NULL });
        result.Should().Be(default(Guid));
    }

    [Fact]
    public void Should_ReturnNull_When_QueryingNullAsNullableGuid()
    {
        var result = SqlMapper.ExecuteScalar<Guid?>(new RequestContext { RealSql = SELECT_NULL });
        result.Should().BeNull();
    }

    [Fact]
    public void Should_ReturnDefaultDateTime_When_QueryingNullAsDateTime()
    {
        var result = SqlMapper.ExecuteScalar<DateTime>(new RequestContext { RealSql = SELECT_NULL });
        result.Should().Be(default(DateTime));
    }

    [Fact]
    public void Should_ReturnNull_When_QueryingNullAsNullableDateTime()
    {
        var result = SqlMapper.ExecuteScalar<DateTime?>(new RequestContext { RealSql = SELECT_NULL });
        result.Should().BeNull();
    }

    [Fact]
    public void Should_ReturnDefaultTimeSpan_When_QueryingNullAsTimeSpan()
    {
        var result = SqlMapper.ExecuteScalar<TimeSpan>(new RequestContext { RealSql = SELECT_NULL });
        result.Should().Be(default(TimeSpan));
    }

    [Fact]
    public void Should_ReturnNull_When_QueryingNullAsNullableTimeSpan()
    {
        var result = SqlMapper.ExecuteScalar<TimeSpan?>(new RequestContext { RealSql = SELECT_NULL });
        result.Should().BeNull();
    }

    [Fact]
    public void Should_ReturnDefaultEnum_When_QueryingNullAsEnum()
    {
        var result = SqlMapper.ExecuteScalar<NumericalEnum>(new RequestContext { RealSql = SELECT_NULL });
        result.Should().Be(default(NumericalEnum));
    }

    [Fact]
    public void Should_ReturnNull_When_QueryingNullAsNullableEnum()
    {
        var result = SqlMapper.ExecuteScalar<NumericalEnum?>(new RequestContext { RealSql = SELECT_NULL });
        result.Should().BeNull();
    }

    [Fact]
    public void Should_ReturnNull_When_QueryingNullAsString()
    {
        var result = SqlMapper.ExecuteScalar<String>(new RequestContext { RealSql = SELECT_NULL });
        result.Should().BeNull();
    }

    #endregion
}
