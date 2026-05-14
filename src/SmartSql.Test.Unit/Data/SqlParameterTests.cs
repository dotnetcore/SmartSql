using System;
using FluentAssertions;
using SmartSql.Data;
using Xunit;

namespace SmartSql.Test.Unit.Data
{
    public class SqlParameterTests
    {
        [Fact]
        public void Should_SetNameAndValue_When_CreatedWithTwoArgs()
        {
            var param = new SqlParameter("Name", "Alice");

            param.Name.Should().Be("Name");
            param.Value.Should().Be("Alice");
            param.ParameterType.Should().Be(typeof(string));
        }

        [Fact]
        public void Should_SetParameterTypeToObject_When_ValueIsNull()
        {
            var param = new SqlParameter("Name", null);

            param.Name.Should().Be("Name");
            param.Value.Should().BeNull();
            param.ParameterType.Should().BeNull();
        }

        [Fact]
        public void Should_SetParameterType_When_CreatedWithThreeArgs()
        {
            var param = new SqlParameter("Count", 42, typeof(int));

            param.Name.Should().Be("Count");
            param.Value.Should().Be(42);
            param.ParameterType.Should().Be(typeof(int));
        }

        [Fact]
        public void Should_DefaultConstructorInitializeWithDefaults()
        {
            var param = new SqlParameter();

            param.Name.Should().BeNull();
            param.Value.Should().BeNull();
            param.ParameterType.Should().BeNull();
            param.SourceParameter.Should().BeNull();
            param.Precision.Should().BeNull();
            param.Scale.Should().BeNull();
            param.Size.Should().BeNull();
            param.DbType.Should().BeNull();
            param.Direction.Should().BeNull();
            param.TypeHandler.Should().BeNull();
        }

        [Fact]
        public void Should_SetSourceParameter_When_Assigned()
        {
            var param = new SqlParameter("Id", 42);

            var dbParam = new Microsoft.Data.Sqlite.SqliteParameter("Id", 42);
            param.SourceParameter = dbParam;

            param.SourceParameter.Should().BeSameAs(dbParam);
            param.SourceParameter.ParameterName.Should().Be("Id");
        }

        [Fact]
        public void Should_UpdateParameterType_When_ValueIsSet()
        {
            var param = new SqlParameter();

            param.Value = "Hello";

            param.ParameterType.Should().Be(typeof(string));
        }

        [Fact]
        public void Should_NotUpdateParameterType_When_ValueIsNull()
        {
            var param = new SqlParameter("Name", "Alice");

            param.Value = null;

            param.ParameterType.Should().Be(typeof(string));
        }

        [Fact]
        public void Should_SetPrecision_When_Assigned()
        {
            var param = new SqlParameter("Amount", 0m);

            param.Precision = 18;

            param.Precision.Should().Be(18);
        }

        [Fact]
        public void Should_SetScale_When_Assigned()
        {
            var param = new SqlParameter("Amount", 0m);

            param.Scale = 2;

            param.Scale.Should().Be(2);
        }

        [Fact]
        public void Should_SetSize_When_Assigned()
        {
            var param = new SqlParameter("Name", "");

            param.Size = 100;

            param.Size.Should().Be(100);
        }

        [Fact]
        public void Should_SetDbType_When_Assigned()
        {
            var param = new SqlParameter("Name", "");

            param.DbType = System.Data.DbType.String;

            param.DbType.Should().Be(System.Data.DbType.String);
        }

        [Fact]
        public void Should_SetDirection_When_Assigned()
        {
            var param = new SqlParameter("Id", 0);

            param.Direction = System.Data.ParameterDirection.Output;

            param.Direction.Should().Be(System.Data.ParameterDirection.Output);
        }
    }
}
