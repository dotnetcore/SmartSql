using System;
using FluentAssertions;
using SmartSql.TypeHandlers;
using Xunit;

namespace SmartSql.Test.Unit.TypeHandlers
{
    /// <summary>
    /// Tests for all cx=2 conversion type handlers (where PropertyType != FieldType).
    /// These handlers convert between numeric types and other field representations.
    /// </summary>
    public class ConversionTypeHandlerTests
    {
        #region Int16ByteTypeHandler

        [Fact]
        public void Should_ReturnInt16_When_Int16ByteTypeHandlerReadsByte()
        {
            var handler = new Int16ByteTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of((byte)42);

            var result = handler.GetValue(reader, 0, typeof(short));

            result.Should().Be((short)42);
        }

        [Fact]
        public void Should_ReturnByte_When_Int16ByteTypeHandlerSetsParameter()
        {
            var handler = new Int16ByteTypeHandler();

            var result = handler.GetSetParameterValue((short)42);

            result.Should().Be((byte)42);
        }

        #endregion

        #region Int32ByteTypeHandler

        [Fact]
        public void Should_ReturnInt32_When_Int32ByteTypeHandlerReadsByte()
        {
            var handler = new Int32ByteTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of((byte)100);

            var result = handler.GetValue(reader, 0, typeof(int));

            result.Should().Be(100);
        }

        [Fact]
        public void Should_ReturnByte_When_Int32ByteTypeHandlerSetsParameter()
        {
            var handler = new Int32ByteTypeHandler();

            var result = handler.GetSetParameterValue(100);

            result.Should().Be((byte)100);
        }

        #endregion

        #region Int32Int16TypeHandler

        [Fact]
        public void Should_ReturnInt32_When_Int32Int16TypeHandlerReadsInt16()
        {
            var handler = new Int32Int16TypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of((short)1234);

            var result = handler.GetValue(reader, 0, typeof(int));

            result.Should().Be(1234);
        }

        [Fact]
        public void Should_ReturnInt16_When_Int32Int16TypeHandlerSetsParameter()
        {
            var handler = new Int32Int16TypeHandler();

            var result = handler.GetSetParameterValue(1234);

            result.Should().Be((short)1234);
        }

        #endregion

        #region Int32Int64TypeHandler

        [Fact]
        public void Should_ReturnInt32_When_Int32Int64TypeHandlerReadsInt64()
        {
            var handler = new Int32Int64TypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of(12345678L);

            var result = handler.GetValue(reader, 0, typeof(int));

            result.Should().Be(12345678);
        }

        [Fact]
        public void Should_ReturnInt64_When_Int32Int64TypeHandlerSetsParameter()
        {
            var handler = new Int32Int64TypeHandler();

            var result = handler.GetSetParameterValue(12345);

            result.Should().Be(12345L);
        }

        #endregion

        #region Int64ByteTypeHandler

        [Fact]
        public void Should_ReturnInt64_When_Int64ByteTypeHandlerReadsByte()
        {
            var handler = new Int64ByteTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of((byte)99);

            var result = handler.GetValue(reader, 0, typeof(long));

            result.Should().Be(99L);
        }

        [Fact]
        public void Should_ReturnByte_When_Int64ByteTypeHandlerSetsParameter()
        {
            var handler = new Int64ByteTypeHandler();

            var result = handler.GetSetParameterValue(99L);

            result.Should().Be((byte)99);
        }

        #endregion

        #region Int64Int16TypeHandler

        [Fact]
        public void Should_ReturnInt64_When_Int64Int16TypeHandlerReadsInt16()
        {
            var handler = new Int64Int16TypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of((short)5000);

            var result = handler.GetValue(reader, 0, typeof(long));

            result.Should().Be(5000L);
        }

        [Fact]
        public void Should_ReturnInt16_When_Int64Int16TypeHandlerSetsParameter()
        {
            var handler = new Int64Int16TypeHandler();

            var result = handler.GetSetParameterValue(5000L);

            result.Should().Be((short)5000);
        }

        #endregion

        #region Int64Int32TypeHandler

        [Fact]
        public void Should_ReturnInt64_When_Int64Int32TypeHandlerReadsInt32()
        {
            var handler = new Int64Int32TypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of(100000);

            var result = handler.GetValue(reader, 0, typeof(long));

            result.Should().Be(100000L);
        }

        [Fact]
        public void Should_ReturnInt32_When_Int64Int32TypeHandlerSetsParameter()
        {
            var handler = new Int64Int32TypeHandler();

            var result = handler.GetSetParameterValue(100000L);

            result.Should().Be(100000);
        }

        #endregion

        #region UInt16ByteTypeHandler

        [Fact]
        public void Should_ReturnUInt16_When_UInt16ByteTypeHandlerReadsByte()
        {
            var handler = new UInt16ByteTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of((byte)200);

            var result = handler.GetValue(reader, 0, typeof(ushort));

            result.Should().Be((ushort)200);
        }

        [Fact]
        public void Should_ReturnByte_When_UInt16ByteTypeHandlerSetsParameter()
        {
            var handler = new UInt16ByteTypeHandler();

            var result = handler.GetSetParameterValue((ushort)200);

            result.Should().Be((byte)200);
        }

        #endregion

        #region UInt32ByteTypeHandler

        [Fact]
        public void Should_ReturnUInt32_When_UInt32ByteTypeHandlerReadsByte()
        {
            var handler = new UInt32ByteTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of((byte)250);

            var result = handler.GetValue(reader, 0, typeof(uint));

            result.Should().Be((uint)250);
        }

        [Fact]
        public void Should_ReturnByte_When_UInt32ByteTypeHandlerSetsParameter()
        {
            var handler = new UInt32ByteTypeHandler();

            var result = handler.GetSetParameterValue((uint)250);

            result.Should().Be((byte)250);
        }

        #endregion

        #region UInt32UInt16TypeHandler

        [Fact]
        public void Should_ReturnUInt32_When_UInt32UInt16TypeHandlerReadsInt16()
        {
            var handler = new UInt32UInt16TypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of((short)1000);

            var result = handler.GetValue(reader, 0, typeof(uint));

            result.Should().Be((uint)1000);
        }

        [Fact]
        public void Should_ReturnUInt16_When_UInt32UInt16TypeHandlerSetsParameter()
        {
            var handler = new UInt32UInt16TypeHandler();

            var result = handler.GetSetParameterValue((uint)1000);

            result.Should().Be((ushort)1000);
        }

        #endregion

        #region UInt64ByteTypeHandler

        [Fact]
        public void Should_ReturnUInt64_When_UInt64ByteTypeHandlerReadsByte()
        {
            var handler = new UInt64ByteTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of((byte)255);

            var result = handler.GetValue(reader, 0, typeof(ulong));

            result.Should().Be((ulong)255);
        }

        [Fact]
        public void Should_ReturnByte_When_UInt64ByteTypeHandlerSetsParameter()
        {
            var handler = new UInt64ByteTypeHandler();

            var result = handler.GetSetParameterValue((ulong)255);

            result.Should().Be((byte)255);
        }

        #endregion

        #region UInt64UInt16TypeHandler

        [Fact]
        public void Should_ReturnUInt64_When_UInt64UInt16TypeHandlerReadsInt16()
        {
            var handler = new UInt64UInt16TypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of((short)30000);

            var result = handler.GetValue(reader, 0, typeof(ulong));

            result.Should().Be((ulong)30000);
        }

        [Fact]
        public void Should_ReturnUInt16_When_UInt64UInt16TypeHandlerSetsParameter()
        {
            var handler = new UInt64UInt16TypeHandler();

            var result = handler.GetSetParameterValue((ulong)30000);

            result.Should().Be((ushort)30000);
        }

        #endregion

        #region UInt64UInt32TypeHandler

        [Fact]
        public void Should_ReturnUInt64_When_UInt64UInt32TypeHandlerReadsInt32()
        {
            var handler = new UInt64UInt32TypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of(500000);

            var result = handler.GetValue(reader, 0, typeof(ulong));

            result.Should().Be((ulong)500000);
        }

        [Fact]
        public void Should_ReturnUInt32_When_UInt64UInt32TypeHandlerSetsParameter()
        {
            var handler = new UInt64UInt32TypeHandler();

            var result = handler.GetSetParameterValue((ulong)500000);

            result.Should().Be((uint)500000);
        }

        #endregion

        #region CharTypeHandler

        [Fact]
        public void Should_ReturnChar_When_CharTypeHandlerReadsString()
        {
            var handler = new CharTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of("A");

            var result = handler.GetValue(reader, 0, typeof(char));

            result.Should().Be('A');
        }

        [Fact]
        public void Should_ReturnString_When_CharTypeHandlerSetsParameter()
        {
            var handler = new CharTypeHandler();

            var result = handler.GetSetParameterValue('Z');

            result.Should().Be("Z");
        }

        #endregion

        #region CharAnyTypeHandler

        [Fact]
        public void Should_ReturnChar_When_CharAnyTypeHandlerReadsString()
        {
            var handler = new CharAnyTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of("B");

            var result = handler.GetValue(reader, 0, typeof(char));

            result.Should().Be('B');
        }

        #endregion

        #region ByteTypeHandler

        [Fact]
        public void Should_ReturnByte_When_ByteTypeHandlerReadsByte()
        {
            var handler = new ByteTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of((byte)128);

            var result = handler.GetValue(reader, 0, typeof(byte));

            result.Should().Be((byte)128);
        }

        #endregion

        #region ByteAnyTypeHandler

        [Fact]
        public void Should_ReturnByte_When_ByteAnyTypeHandlerReadsObject()
        {
            var handler = new ByteAnyTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of(64);

            var result = handler.GetValue(reader, 0, typeof(byte));

            result.Should().Be((byte)64);
        }

        #endregion

        #region Int16TypeHandler

        [Fact]
        public void Should_ReturnInt16_When_Int16TypeHandlerReadsInt16()
        {
            var handler = new Int16TypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of((short)1000);

            var result = handler.GetValue(reader, 0, typeof(short));

            result.Should().Be((short)1000);
        }

        #endregion

        #region Int16AnyTypeHandler

        [Fact]
        public void Should_ReturnInt16_When_Int16AnyTypeHandlerReadsObject()
        {
            var handler = new Int16AnyTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of(500);

            var result = handler.GetValue(reader, 0, typeof(short));

            result.Should().Be((short)500);
        }

        #endregion

        #region Int32TypeHandler

        [Fact]
        public void Should_ReturnInt32_When_Int32TypeHandlerReadsInt32()
        {
            var handler = new Int32TypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of(42);

            var result = handler.GetValue(reader, 0, typeof(int));

            result.Should().Be(42);
        }

        #endregion

        #region Int32AnyTypeHandler

        [Fact]
        public void Should_ReturnInt32_When_Int32AnyTypeHandlerReadsObject()
        {
            var handler = new Int32AnyTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of(99L);

            var result = handler.GetValue(reader, 0, typeof(int));

            result.Should().Be(99);
        }

        #endregion

        #region Int64TypeHandler

        [Fact]
        public void Should_ReturnInt64_When_Int64TypeHandlerReadsInt64()
        {
            var handler = new Int64TypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of(123456789L);

            var result = handler.GetValue(reader, 0, typeof(long));

            result.Should().Be(123456789L);
        }

        #endregion

        #region Int64AnyTypeHandler

        [Fact]
        public void Should_ReturnInt64_When_Int64AnyTypeHandlerReadsObject()
        {
            var handler = new Int64AnyTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of(999);

            var result = handler.GetValue(reader, 0, typeof(long));

            result.Should().Be(999L);
        }

        #endregion

        #region UInt16TypeHandler

        [Fact]
        public void Should_ReturnUInt16_When_UInt16TypeHandlerReadsObject()
        {
            var handler = new UInt16TypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of((ushort)1000);

            var result = handler.GetValue(reader, 0, typeof(ushort));

            result.Should().Be((ushort)1000);
        }

        #endregion

        #region UInt16AnyTypeHandler

        [Fact]
        public void Should_ReturnUInt16_When_UInt16AnyTypeHandlerReadsObject()
        {
            var handler = new UInt16AnyTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of(800);

            var result = handler.GetValue(reader, 0, typeof(ushort));

            result.Should().Be((ushort)800);
        }

        #endregion

        #region UInt32TypeHandler

        [Fact]
        public void Should_ReturnUInt32_When_UInt32TypeHandlerReadsObject()
        {
            var handler = new UInt32TypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of((uint)500000);

            var result = handler.GetValue(reader, 0, typeof(uint));

            result.Should().Be((uint)500000);
        }

        #endregion

        #region UInt32AnyTypeHandler

        [Fact]
        public void Should_ReturnUInt32_When_UInt32AnyTypeHandlerReadsObject()
        {
            var handler = new UInt32AnyTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of(400000L);

            var result = handler.GetValue(reader, 0, typeof(uint));

            result.Should().Be((uint)400000);
        }

        #endregion

        #region UInt64TypeHandler

        [Fact]
        public void Should_ReturnUInt64_When_UInt64TypeHandlerReadsObject()
        {
            var handler = new UInt64TypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of((ulong)999999);

            var result = handler.GetValue(reader, 0, typeof(ulong));

            result.Should().Be((ulong)999999);
        }

        #endregion

        #region UInt64AnyTypeHandler

        [Fact]
        public void Should_ReturnUInt64_When_UInt64AnyTypeHandlerReadsObject()
        {
            var handler = new UInt64AnyTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of(888888L);

            var result = handler.GetValue(reader, 0, typeof(ulong));

            result.Should().Be((ulong)888888);
        }

        #endregion

        #region DecimalTypeHandler

        [Fact]
        public void Should_ReturnDecimal_When_DecimalTypeHandlerReadsDecimal()
        {
            var handler = new DecimalTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of(123.45m);

            var result = handler.GetValue(reader, 0, typeof(decimal));

            result.Should().Be(123.45m);
        }

        #endregion

        #region DecimalAnyTypeHandler

        [Fact]
        public void Should_ReturnDecimal_When_DecimalAnyTypeHandlerReadsObject()
        {
            var handler = new DecimalAnyTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of(67.89);

            var result = handler.GetValue(reader, 0, typeof(decimal));

            result.Should().Be(67.89m);
        }

        #endregion

        #region DoubleTypeHandler

        [Fact]
        public void Should_ReturnDouble_When_DoubleTypeHandlerReadsDouble()
        {
            var handler = new DoubleTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of(3.14);

            var result = handler.GetValue(reader, 0, typeof(double));

            result.Should().Be(3.14);
        }

        #endregion

        #region DoubleAnyTypeHandler

        [Fact]
        public void Should_ReturnDouble_When_DoubleAnyTypeHandlerReadsObject()
        {
            var handler = new DoubleAnyTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of(2.718f);

            var result = handler.GetValue(reader, 0, typeof(double));

            result.Should().BeApproximately(2.718, 0.001);
        }

        #endregion

        #region BooleanTypeHandler

        [Fact]
        public void Should_ReturnBoolean_When_BooleanTypeHandlerReadsBoolean()
        {
            var handler = new BooleanTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of(true);

            var result = handler.GetValue(reader, 0, typeof(bool));

            result.Should().BeTrue();
        }

        #endregion

        #region BooleanAnyTypeHandler

        [Fact]
        public void Should_ReturnBoolean_When_BooleanAnyTypeHandlerReadsObject()
        {
            var handler = new BooleanAnyTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of(1);

            var result = handler.GetValue(reader, 0, typeof(bool));

            result.Should().BeTrue();
        }

        #endregion

        #region GuidTypeHandler

        [Fact]
        public void Should_ReturnGuid_When_GuidTypeHandlerReadsGuid()
        {
            var expected = Guid.NewGuid();
            var handler = new GuidTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of(expected);

            var result = handler.GetValue(reader, 0, typeof(Guid));

            result.Should().Be(expected);
        }

        #endregion

        #region GuidAnyTypeHandler

        [Fact]
        public void Should_ReturnGuid_When_GuidAnyTypeHandlerReadsObject()
        {
            var expected = Guid.NewGuid();
            var handler = new GuidAnyTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of(expected.ToString());

            var result = handler.GetValue(reader, 0, typeof(Guid));

            result.Should().Be(expected);
        }

        #endregion

        #region DateTimeTypeHandler

        [Fact]
        public void Should_ReturnDateTime_When_DateTimeTypeHandlerReadsDateTime()
        {
            var expected = new DateTime(2024, 6, 15, 12, 30, 0);
            var handler = new DateTimeTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of(expected);

            var result = handler.GetValue(reader, 0, typeof(DateTime));

            result.Should().Be(expected);
        }

        #endregion

        #region DateTimeAnyTypeHandler

        [Fact]
        public void Should_ReturnDateTime_When_DateTimeAnyTypeHandlerReadsObject()
        {
            var expected = new DateTime(2024, 3, 20);
            var handler = new DateTimeAnyTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of(expected);

            var result = handler.GetValue(reader, 0, typeof(DateTime));

            result.Should().Be(expected);
        }

        #endregion

        #region SingleTypeHandler

        [Fact]
        public void Should_ReturnFloat_When_SingleTypeHandlerReadsFloat()
        {
            var handler = new SingleTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of(3.14f);

            var result = handler.GetValue(reader, 0, typeof(float));

            result.Should().BeApproximately(3.14f, 0.001f);
        }

        #endregion

        #region SingleAnyTypeHandler

        [Fact]
        public void Should_ReturnFloat_When_SingleAnyTypeHandlerReadsObject()
        {
            var handler = new SingleAnyTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of(2.5);

            var result = handler.GetValue(reader, 0, typeof(float));

            result.Should().BeApproximately(2.5f, 0.001f);
        }

        #endregion

        #region SByteTypeHandler

        [Fact]
        public void Should_ReturnSByte_When_SByteTypeHandlerReadsObject()
        {
            var handler = new SByteTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of((sbyte)-10);

            var result = handler.GetValue(reader, 0, typeof(sbyte));

            result.Should().Be((sbyte)-10);
        }

        #endregion

        #region Nullable type handlers (GetValue tests with null and non-null)

        [Fact]
        public void Should_ReturnNull_When_NullableInt16TypeHandlerReadsNull()
        {
            var handler = new NullableInt16TypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.OfNull();

            var result = handler.GetValue(reader, 0, typeof(short?));

            result.Should().BeNull();
        }

        [Fact]
        public void Should_ReturnInt16_When_NullableInt16TypeHandlerReadsNonNull()
        {
            var handler = new NullableInt16TypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of((short)100);

            var result = handler.GetValue(reader, 0, typeof(short?));

            result.Should().Be((short)100);
        }

        [Fact]
        public void Should_ReturnNull_When_NullableInt32TypeHandlerReadsNull()
        {
            var handler = new NullableInt32TypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.OfNull();

            var result = handler.GetValue(reader, 0, typeof(int?));

            result.Should().BeNull();
        }

        [Fact]
        public void Should_ReturnInt32_When_NullableInt32TypeHandlerReadsNonNull()
        {
            var handler = new NullableInt32TypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of(42);

            var result = handler.GetValue(reader, 0, typeof(int?));

            result.Should().Be(42);
        }

        [Fact]
        public void Should_ReturnNull_When_NullableInt64TypeHandlerReadsNull()
        {
            var handler = new NullableInt64TypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.OfNull();

            var result = handler.GetValue(reader, 0, typeof(long?));

            result.Should().BeNull();
        }

        [Fact]
        public void Should_ReturnInt64_When_NullableInt64TypeHandlerReadsNonNull()
        {
            var handler = new NullableInt64TypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of(123456L);

            var result = handler.GetValue(reader, 0, typeof(long?));

            result.Should().Be(123456L);
        }

        [Fact]
        public void Should_ReturnNull_When_NullableByteTypeHandlerReadsNull()
        {
            var handler = new NullableByteTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.OfNull();

            var result = handler.GetValue(reader, 0, typeof(byte?));

            result.Should().BeNull();
        }

        [Fact]
        public void Should_ReturnByte_When_NullableByteTypeHandlerReadsNonNull()
        {
            var handler = new NullableByteTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of((byte)200);

            var result = handler.GetValue(reader, 0, typeof(byte?));

            result.Should().Be((byte)200);
        }

        [Fact]
        public void Should_ReturnNull_When_NullableDecimalTypeHandlerReadsNull()
        {
            var handler = new NullableDecimalTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.OfNull();

            var result = handler.GetValue(reader, 0, typeof(decimal?));

            result.Should().BeNull();
        }

        [Fact]
        public void Should_ReturnDecimal_When_NullableDecimalTypeHandlerReadsNonNull()
        {
            var handler = new NullableDecimalTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of(99.99m);

            var result = handler.GetValue(reader, 0, typeof(decimal?));

            result.Should().Be(99.99m);
        }

        [Fact]
        public void Should_ReturnNull_When_NullableDoubleTypeHandlerReadsNull()
        {
            var handler = new NullableDoubleTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.OfNull();

            var result = handler.GetValue(reader, 0, typeof(double?));

            result.Should().BeNull();
        }

        [Fact]
        public void Should_ReturnDouble_When_NullableDoubleTypeHandlerReadsNonNull()
        {
            var handler = new NullableDoubleTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of(3.14);

            var result = handler.GetValue(reader, 0, typeof(double?));

            result.Should().Be(3.14);
        }

        [Fact]
        public void Should_ReturnNull_When_NullableSingleTypeHandlerReadsNull()
        {
            var handler = new NullableSingleTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.OfNull();

            var result = handler.GetValue(reader, 0, typeof(float?));

            result.Should().BeNull();
        }

        [Fact]
        public void Should_ReturnFloat_When_NullableSingleTypeHandlerReadsNonNull()
        {
            var handler = new NullableSingleTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of(1.5f);

            var result = handler.GetValue(reader, 0, typeof(float?));

            result.Should().BeApproximately(1.5f, 0.001f);
        }

        [Fact]
        public void Should_ReturnNull_When_NullableDateTimeTypeHandlerReadsNull()
        {
            var handler = new NullableDateTimeTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.OfNull();

            var result = handler.GetValue(reader, 0, typeof(DateTime?));

            result.Should().BeNull();
        }

        [Fact]
        public void Should_ReturnDateTime_When_NullableDateTimeTypeHandlerReadsNonNull()
        {
            var expected = new DateTime(2024, 1, 1);
            var handler = new NullableDateTimeTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of(expected);

            var result = handler.GetValue(reader, 0, typeof(DateTime?));

            result.Should().Be(expected);
        }

        [Fact]
        public void Should_ReturnNull_When_NullableGuidTypeHandlerReadsNull()
        {
            var handler = new NullableGuidTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.OfNull();

            var result = handler.GetValue(reader, 0, typeof(Guid?));

            result.Should().BeNull();
        }

        [Fact]
        public void Should_ReturnGuid_When_NullableGuidTypeHandlerReadsNonNull()
        {
            var expected = Guid.NewGuid();
            var handler = new NullableGuidTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of(expected);

            var result = handler.GetValue(reader, 0, typeof(Guid?));

            result.Should().Be(expected);
        }

        [Fact]
        public void Should_ReturnNull_When_NullableUInt16TypeHandlerReadsNull()
        {
            var handler = new NullableUInt16TypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.OfNull();

            var result = handler.GetValue(reader, 0, typeof(ushort?));

            result.Should().BeNull();
        }

        [Fact]
        public void Should_ReturnUInt16_When_NullableUInt16TypeHandlerReadsNonNull()
        {
            var handler = new NullableUInt16TypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of((ushort)500);

            var result = handler.GetValue(reader, 0, typeof(ushort?));

            result.Should().Be((ushort)500);
        }

        [Fact]
        public void Should_ReturnNull_When_NullableUInt32TypeHandlerReadsNull()
        {
            var handler = new NullableUInt32TypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.OfNull();

            var result = handler.GetValue(reader, 0, typeof(uint?));

            result.Should().BeNull();
        }

        [Fact]
        public void Should_ReturnUInt32_When_NullableUInt32TypeHandlerReadsNonNull()
        {
            var handler = new NullableUInt32TypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of((uint)100000);

            var result = handler.GetValue(reader, 0, typeof(uint?));

            result.Should().Be((uint)100000);
        }

        [Fact]
        public void Should_ReturnNull_When_NullableUInt64TypeHandlerReadsNull()
        {
            var handler = new NullableUInt64TypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.OfNull();

            var result = handler.GetValue(reader, 0, typeof(ulong?));

            result.Should().BeNull();
        }

        [Fact]
        public void Should_ReturnUInt64_When_NullableUInt64TypeHandlerReadsNonNull()
        {
            var handler = new NullableUInt64TypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of((ulong)999999);

            var result = handler.GetValue(reader, 0, typeof(ulong?));

            result.Should().Be((ulong)999999);
        }

        [Fact]
        public void Should_ReturnNull_When_NullableSByteTypeHandlerReadsNull()
        {
            var handler = new NullableSByteTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.OfNull();

            var result = handler.GetValue(reader, 0, typeof(sbyte?));

            result.Should().BeNull();
        }

        [Fact]
        public void Should_ReturnSByte_When_NullableSByteTypeHandlerReadsNonNull()
        {
            var handler = new NullableSByteTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of((sbyte)-5);

            var result = handler.GetValue(reader, 0, typeof(sbyte?));

            result.Should().Be((sbyte)-5);
        }

        #endregion

        #region Nullable Any type handlers

        [Fact]
        public void Should_ReturnInt16_When_NullableInt16AnyTypeHandlerReadsNonNull()
        {
            var handler = new NullableInt16AnyTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of(300);

            var result = handler.GetValue(reader, 0, typeof(short?));

            result.Should().Be((short)300);
        }

        [Fact]
        public void Should_ReturnInt32_When_NullableInt32AnyTypeHandlerReadsNonNull()
        {
            var handler = new NullableInt32AnyTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of(70000L);

            var result = handler.GetValue(reader, 0, typeof(int?));

            result.Should().Be(70000);
        }

        [Fact]
        public void Should_ReturnInt64_When_NullableInt64AnyTypeHandlerReadsNonNull()
        {
            var handler = new NullableInt64AnyTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of(999999L);

            var result = handler.GetValue(reader, 0, typeof(long?));

            result.Should().Be(999999L);
        }

        [Fact]
        public void Should_ReturnByte_When_NullableByteAnyTypeHandlerReadsNonNull()
        {
            var handler = new NullableByteAnyTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of(100);

            var result = handler.GetValue(reader, 0, typeof(byte?));

            result.Should().Be((byte)100);
        }

        [Fact]
        public void Should_ReturnDecimal_When_NullableDecimalAnyTypeHandlerReadsNonNull()
        {
            var handler = new NullableDecimalAnyTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of(55.5);

            var result = handler.GetValue(reader, 0, typeof(decimal?));

            result.Should().Be(55.5m);
        }

        [Fact]
        public void Should_ReturnDouble_When_NullableDoubleAnyTypeHandlerReadsNonNull()
        {
            var handler = new NullableDoubleAnyTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of(1.1f);

            var result = handler.GetValue(reader, 0, typeof(double?));

            result.Should().BeApproximately(1.1, 0.001);
        }

        [Fact]
        public void Should_ReturnFloat_When_NullableSingleAnyTypeHandlerReadsNonNull()
        {
            var handler = new NullableSingleAnyTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of(4.5);

            var result = handler.GetValue(reader, 0, typeof(float?));

            result.Should().BeApproximately(4.5f, 0.001f);
        }

        [Fact]
        public void Should_ReturnDateTime_When_NullableDateTimeAnyTypeHandlerReadsNonNull()
        {
            var expected = new DateTime(2025, 12, 25);
            var handler = new NullableDateTimeAnyTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of(expected);

            var result = handler.GetValue(reader, 0, typeof(DateTime?));

            result.Should().Be(expected);
        }

        [Fact]
        public void Should_ReturnGuid_When_NullableGuidAnyTypeHandlerReadsNonNull()
        {
            var expected = Guid.NewGuid();
            var handler = new NullableGuidAnyTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of(expected.ToString());

            var result = handler.GetValue(reader, 0, typeof(Guid?));

            result.Should().Be(expected);
        }

        [Fact]
        public void Should_ReturnUInt16_When_NullableUInt16AnyTypeHandlerReadsNonNull()
        {
            var handler = new NullableUInt16AnyTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of((ushort)600);

            var result = handler.GetValue(reader, 0, typeof(ushort?));

            result.Should().Be((ushort)600);
        }

        [Fact]
        public void Should_ReturnUInt32_When_NullableUInt32AnyTypeHandlerReadsNonNull()
        {
            var handler = new NullableUInt32AnyTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of((uint)200000);

            var result = handler.GetValue(reader, 0, typeof(uint?));

            result.Should().Be((uint)200000);
        }

        [Fact]
        public void Should_ReturnUInt64_When_NullableUInt64AnyTypeHandlerReadsNonNull()
        {
            var handler = new NullableUInt64AnyTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of((ulong)777777);

            var result = handler.GetValue(reader, 0, typeof(ulong?));

            result.Should().Be((ulong)777777);
        }

        #endregion

        #region StringTypeHandler

        [Fact]
        public void Should_ReturnNull_When_StringTypeHandlerReadsNull()
        {
            var handler = new StringTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.OfNull();

            var result = handler.GetValue(reader, 0, typeof(string));

            result.Should().BeNull();
        }

        [Fact]
        public void Should_ReturnString_When_StringTypeHandlerReadsNonNull()
        {
            var handler = new StringTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of("hello");

            var result = handler.GetValue(reader, 0, typeof(string));

            result.Should().Be("hello");
        }

        #endregion

        #region StringAnyTypeHandler

        [Fact]
        public void Should_ReturnNull_When_StringAnyTypeHandlerReadsNull()
        {
            var handler = new StringAnyTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.OfNull();

            var result = handler.GetValue(reader, 0, typeof(string));

            result.Should().BeNull();
        }

        [Fact]
        public void Should_ReturnString_When_StringAnyTypeHandlerReadsNonNull()
        {
            var handler = new StringAnyTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of(123);

            var result = handler.GetValue(reader, 0, typeof(string));

            result.Should().Be("123");
        }

        #endregion

        #region NullableCharTypeHandler

        [Fact]
        public void Should_ReturnNull_When_NullableCharTypeHandlerReadsNull()
        {
            var handler = new NullableCharTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.OfNull();

            var result = handler.GetValue(reader, 0, typeof(char?));

            result.Should().BeNull();
        }

        [Fact]
        public void Should_ReturnChar_When_NullableCharTypeHandlerReadsNonNull()
        {
            var handler = new NullableCharTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of("X");

            var result = handler.GetValue(reader, 0, typeof(char?));

            result.Should().Be('X');
        }

        [Fact]
        public void Should_ReturnString_When_NullableCharTypeHandlerSetsNonNullParameter()
        {
            var handler = new NullableCharTypeHandler();

            var result = handler.GetSetParameterValue('A');

            result.Should().Be("A");
        }

        [Fact]
        public void Should_ReturnDBNull_When_NullableCharTypeHandlerSetsNullParameter()
        {
            var handler = new NullableCharTypeHandler();

            var result = handler.GetSetParameterValue(null);

            result.Should().Be(DBNull.Value);
        }

        #endregion

        #region NullableCharAnyTypeHandler

        [Fact]
        public void Should_ReturnNull_When_NullableCharAnyTypeHandlerReadsNull()
        {
            var handler = new NullableCharAnyTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.OfNull();

            var result = handler.GetValue(reader, 0, typeof(char?));

            result.Should().BeNull();
        }

        [Fact]
        public void Should_ReturnChar_When_NullableCharAnyTypeHandlerReadsNonNull()
        {
            var handler = new NullableCharAnyTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of('Y');

            var result = handler.GetValue(reader, 0, typeof(char?));

            result.Should().Be('Y');
        }

        #endregion

        #region NullableBooleanTypeHandler

        [Fact]
        public void Should_ReturnNull_When_NullableBooleanTypeHandlerReadsNull()
        {
            var handler = new NullableBooleanTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.OfNull();

            var result = handler.GetValue(reader, 0, typeof(bool?));

            result.Should().BeNull();
        }

        [Fact]
        public void Should_ReturnBoolean_When_NullableBooleanTypeHandlerReadsNonNull()
        {
            var handler = new NullableBooleanTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of(true);

            var result = handler.GetValue(reader, 0, typeof(bool?));

            result.Should().BeTrue();
        }

        #endregion

        #region NullableBooleanAnyTypeHandler

        [Fact]
        public void Should_ReturnBoolean_When_NullableBooleanAnyTypeHandlerReadsNonNull()
        {
            var handler = new NullableBooleanAnyTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of(1);

            var result = handler.GetValue(reader, 0, typeof(bool?));

            result.Should().BeTrue();
        }

        #endregion

        #region UnknownTypeHandler

        [Fact]
        public void Should_ReturnNull_When_UnknownTypeHandlerReadsNull()
        {
            var handler = new UnknownTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.OfNull();

            var result = handler.GetValue(reader, 0, typeof(object));

            result.Should().BeNull();
        }

        [Fact]
        public void Should_ReturnObject_When_UnknownTypeHandlerReadsNonNull()
        {
            var handler = new UnknownTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of("some value");

            var result = handler.GetValue(reader, 0, typeof(object));

            result.Should().Be("some value");
        }

        #endregion
    }
}
