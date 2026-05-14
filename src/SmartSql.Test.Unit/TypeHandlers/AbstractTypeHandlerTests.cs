using System;
using System.Collections.Generic;
using System.Data;
using FluentAssertions;
using Moq;
using SmartSql.Data;
using SmartSql.TypeHandlers;
using Xunit;

namespace SmartSql.Test.Unit.TypeHandlers
{
    public class AbstractTypeHandlerTests
    {
        [Fact]
        public void PropertyType_Should_BeSetCorrectly_When_HandlerCreated()
        {
            var handler = new TestTypeHandler();

            handler.PropertyType.Should().Be(typeof(string));
            handler.FieldType.Should().Be(typeof(string));
        }

        [Fact]
        public void Default_Should_BeNull_When_PropertyTypeIsReferenceType()
        {
            var handler = new TestTypeHandler();

            handler.Default.Should().BeNull();
        }

        [Fact]
        public void Default_Should_BeZero_When_PropertyTypeIsInt()
        {
            var handler = new TestIntTypeHandler();

            handler.Default.Should().Be(0);
        }

        [Fact]
        public void IsNullable_Should_BeTrue_When_PropertyTypeIsReferenceType()
        {
            var handler = new TestTypeHandler();

            handler.IsNullable.Should().BeTrue();
        }

        [Fact]
        public void IsNullable_Should_BeFalse_When_PropertyTypeIsValueType()
        {
            var handler = new TestIntTypeHandler();

            handler.IsNullable.Should().BeFalse();
        }

        [Fact]
        public void Initialize_Should_SetDbType_When_ParameterContainsDbType()
        {
            var handler = new TestTypeHandler();

            handler.Initialize(new Dictionary<string, object>
            {
                { "DbType", "AnsiString" }
            });

            handler.DbType.Should().Be(DbType.AnsiString);
        }

        [Fact]
        public void Initialize_Should_NotSetDbType_When_ParameterDoesNotContainDbType()
        {
            var handler = new TestTypeHandler();

            handler.Initialize(new Dictionary<string, object>
            {
                { "OtherProp", "value" }
            });

            handler.DbType.Should().BeNull();
        }

        [Fact]
        public void Initialize_Should_NotThrow_When_ParametersIsNull()
        {
            var handler = new TestTypeHandler();

            var act = () => handler.Initialize(null);

            act.Should().NotThrow();
        }

        [Fact]
        public void Initialize_Should_NotSetDbType_When_DbTypeValueIsInvalid()
        {
            var handler = new TestTypeHandler();

            handler.Initialize(new Dictionary<string, object>
            {
                { "DbType", "NonExistentDbType" }
            });

            handler.DbType.Should().BeNull();
        }

        [Fact]
        public void Initialize_Should_ParseEnumCaseInsensitively_When_DbTypeKeyIsCorrect()
        {
            var handler = new TestTypeHandler();

            handler.Initialize(new Dictionary<string, object>
            {
                { "DbType", "string" }
            });

            handler.DbType.Should().Be(DbType.String);
        }

        [Fact]
        public void GetSetParameterValue_Should_ReturnDBNull_When_NullAndNullable()
        {
            var handler = new TestTypeHandler();

            var result = handler.GetSetParameterValue(null);

            result.Should().Be(DBNull.Value);
        }

        [Fact]
        public void GetSetParameterValue_Should_ReturnDefault_When_NullAndNotNullable()
        {
            var handler = new TestIntTypeHandler();

            var result = handler.GetSetParameterValue(null);

            result.Should().Be(0);
        }

        [Fact]
        public void GetSetParameterValue_Should_ReturnValue_When_NotNull()
        {
            var handler = new TestTypeHandler();

            var result = handler.GetSetParameterValue("hello");

            result.Should().Be("hello");
        }

        [Fact]
        public void SetParameter_Should_SetValue_When_NoDbType()
        {
            var handler = new TestTypeHandler();
            var mockParam = new Mock<IDataParameter>();

            handler.SetParameter(mockParam.Object, "test_value");

            mockParam.VerifySet(p => p.Value = "test_value", Times.Once);
            mockParam.VerifySet(p => p.DbType = It.IsAny<DbType>(), Times.Never);
        }

        [Fact]
        public void SetParameter_Should_SetDbTypeAndValue_When_DbTypeConfigured()
        {
            var handler = new TestTypeHandler();
            handler.Initialize(new Dictionary<string, object>
            {
                { "DbType", "AnsiString" }
            });
            var mockParam = new Mock<IDataParameter>();
            mockParam.SetupProperty(p => p.DbType);
            mockParam.SetupProperty(p => p.Value);

            handler.SetParameter(mockParam.Object, "test_value");

            mockParam.Object.DbType.Should().Be(DbType.AnsiString);
            mockParam.Object.Value.Should().Be("test_value");
        }

        [Fact]
        public void SetParameter_Should_SetDBNull_When_NullValueAndNullable()
        {
            var handler = new TestTypeHandler();
            var mockParam = new Mock<IDataParameter>();
            mockParam.SetupProperty(p => p.Value);

            handler.SetParameter(mockParam.Object, null);

            mockParam.Object.Value.Should().Be(DBNull.Value);
        }

        [Fact]
        public void GetSetParameterValueWhenNotNull_Should_ReturnSameValue_ByDefault()
        {
            var handler = new TestTypeHandler();

            var result = handler.GetSetParameterValue("hello");

            result.Should().Be("hello");
        }

        [Fact]
        public void GetSetParameterValueWhenNotNull_Should_ReturnZero_ForDefaults()
        {
            var handler = new TestIntTypeHandler();

            var result = handler.GetSetParameterValue(42);

            result.Should().Be(42);
        }

        [Fact]
        public void GetValue_Should_ReturnCorrectValue_When_HandlerQueriesReader()
        {
            var handler = new TestTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of("expected_value");

            var result = handler.GetValue(reader, 0, typeof(string));

            result.Should().Be("expected_value");
        }

        [Fact]
        public void GetValue_Should_ReturnNull_When_NullValueFromReader()
        {
            var handler = new TestTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of(null);

            var result = handler.GetValue(reader, 0, typeof(string));

            result.Should().BeNull();
        }

        public class TestTypeHandler : AbstractTypeHandler<string, string>
        {
            public override string GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
            {
                return dataReader.GetString(columnIndex);
            }
        }

        public class TestIntTypeHandler : AbstractTypeHandler<int, int>
        {
            public override int GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
            {
                return dataReader.GetInt32(columnIndex);
            }
        }
    }
}
