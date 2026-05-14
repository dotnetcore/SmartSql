using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SmartSql.Data;
using Xunit;

namespace SmartSql.Test.Unit.Data
{
    public class DataReaderWrapperDeepTests
    {
        private readonly Mock<DbDataReader> _mockReader;
        private readonly DataReaderWrapper _wrapper;

        public DataReaderWrapperDeepTests()
        {
            _mockReader = new Mock<DbDataReader>();
            _wrapper = new DataReaderWrapper(_mockReader.Object);
        }

        [Fact]
        public void Should_DelegateGetBytes_When_Called()
        {
            var buffer = new byte[100];
            _mockReader.Setup(r => r.GetBytes(0, 0, It.IsAny<byte[]>(), 0, 100))
                .Returns(50L);

            var result = _wrapper.GetBytes(0, 0, buffer, 0, 100);

            result.Should().Be(50L);
            _mockReader.Verify(r => r.GetBytes(0, 0, buffer, 0, 100), Times.Once);
        }

        [Fact]
        public void Should_DelegateGetChars_When_Called()
        {
            var buffer = new char[100];
            _mockReader.Setup(r => r.GetChars(0, 0, It.IsAny<char[]>(), 0, 100))
                .Returns(10L);

            var result = _wrapper.GetChars(0, 0, buffer, 0, 100);

            result.Should().Be(10L);
            _mockReader.Verify(r => r.GetChars(0, 0, buffer, 0, 100), Times.Once);
        }

        [Fact]
        public void Should_DelegateGetDataTypeName_When_Called()
        {
            _mockReader.Setup(r => r.GetDataTypeName(0)).Returns("bigint");

            var result = _wrapper.GetDataTypeName(0);

            result.Should().Be("bigint");
            _mockReader.Verify(r => r.GetDataTypeName(0), Times.Once);
        }

        [Fact]
        public void Should_DelegateGetBoolean_When_Called()
        {
            _mockReader.Setup(r => r.GetBoolean(0)).Returns(true);

            var result = _wrapper.GetBoolean(0);

            result.Should().BeTrue();
            _mockReader.Verify(r => r.GetBoolean(0), Times.Once);
        }

        [Fact]
        public void Should_DelegateGetByte_When_Called()
        {
            _mockReader.Setup(r => r.GetByte(0)).Returns((byte)5);

            var result = _wrapper.GetByte(0);

            result.Should().Be(5);
            _mockReader.Verify(r => r.GetByte(0), Times.Once);
        }

        [Fact]
        public void Should_DelegateGetChar_When_Called()
        {
            _mockReader.Setup(r => r.GetChar(0)).Returns('A');

            var result = _wrapper.GetChar(0);

            result.Should().Be('A');
            _mockReader.Verify(r => r.GetChar(0), Times.Once);
        }

        [Fact]
        public void Should_DelegateGetDateTime_When_Called()
        {
            var dt = new DateTime(2026, 5, 14);
            _mockReader.Setup(r => r.GetDateTime(0)).Returns(dt);

            var result = _wrapper.GetDateTime(0);

            result.Should().Be(dt);
            _mockReader.Verify(r => r.GetDateTime(0), Times.Once);
        }

        [Fact]
        public void Should_DelegateGetDecimal_When_Called()
        {
            _mockReader.Setup(r => r.GetDecimal(0)).Returns(1.5m);

            var result = _wrapper.GetDecimal(0);

            result.Should().Be(1.5m);
            _mockReader.Verify(r => r.GetDecimal(0), Times.Once);
        }

        [Fact]
        public void Should_DelegateGetDouble_When_Called()
        {
            _mockReader.Setup(r => r.GetDouble(0)).Returns(3.14);

            var result = _wrapper.GetDouble(0);

            result.Should().Be(3.14);
            _mockReader.Verify(r => r.GetDouble(0), Times.Once);
        }

        [Fact]
        public void Should_DelegateGetFloat_When_Called()
        {
            _mockReader.Setup(r => r.GetFloat(0)).Returns(2.5f);

            var result = _wrapper.GetFloat(0);

            result.Should().Be(2.5f);
        }

        [Fact]
        public void Should_DelegateGetGuid_When_Called()
        {
            var guid = Guid.NewGuid();
            _mockReader.Setup(r => r.GetGuid(0)).Returns(guid);

            var result = _wrapper.GetGuid(0);

            result.Should().Be(guid);
        }

        [Fact]
        public void Should_DelegateGetInt16_When_Called()
        {
            _mockReader.Setup(r => r.GetInt16(0)).Returns((short)100);

            var result = _wrapper.GetInt16(0);

            result.Should().Be(100);
        }

        [Fact]
        public void Should_DelegateGetOrdinal_When_Called()
        {
            _mockReader.Setup(r => r.GetOrdinal("column_name")).Returns(3);

            var result = _wrapper.GetOrdinal("column_name");

            result.Should().Be(3);
        }

        [Fact]
        public void Should_DelegateGetValues_When_Called()
        {
            var values = new object[2];
            _mockReader.Setup(r => r.GetValues(It.IsAny<object[]>())).Returns(2);

            var result = _wrapper.GetValues(values);

            result.Should().Be(2);
        }

        [Fact]
        public void Should_DelegateGetEnumerator_When_Called()
        {
            var enumerator = new System.Collections.ArrayList { 1, 2 }.GetEnumerator();
            _mockReader.Setup(r => r.GetEnumerator()).Returns(enumerator);

            var result = _wrapper.GetEnumerator();

            result.Should().BeSameAs(enumerator);
        }

        [Fact]
        public void Should_DelegateClose_When_Called()
        {
            _wrapper.Close();

            _mockReader.Verify(r => r.Close(), Times.Once);
        }

        [Fact]
        public void Should_ReturnLegacyRead_When_Called()
        {
            _mockReader.Setup(r => r.Read()).Returns(true);

            var result = _wrapper.Read();

            result.Should().BeTrue();
        }

        [Fact]
        public async Task Should_ReturnReadAsync_When_Called()
        {
            _mockReader.Setup(r => r.ReadAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var result = await _wrapper.ReadAsync(CancellationToken.None);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task Should_ReturnIsDBNullAsync_When_Called()
        {
            _mockReader.Setup(r => r.IsDBNullAsync(0, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var result = await _wrapper.IsDBNullAsync(0, CancellationToken.None);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_DelegateGetFieldValue_When_Called()
        {
            _mockReader.Setup(r => r.GetFieldValue<string>(0)).Returns("hello");

            var result = _wrapper.GetFieldValue<string>(0);

            result.Should().Be("hello");
        }

        [Fact]
        public async Task Should_DelegateGetFieldValueAsync_When_Called()
        {
            _mockReader.Setup(r => r.GetFieldValueAsync<string>(0, It.IsAny<CancellationToken>())).ReturnsAsync("world");

            var result = await _wrapper.GetFieldValueAsync<string>(0, CancellationToken.None);

            result.Should().Be("world");
        }

        [Fact]
        public void Should_DelegateGetHashCode_When_Called()
        {
            _mockReader.Setup(r => r.GetHashCode()).Returns(42);

            var result = _wrapper.GetHashCode();

            result.Should().Be(42);
        }

        [Fact]
        public void Should_DelegateEquals_When_Called()
        {
            _mockReader.Setup(r => r.Equals(It.IsAny<object>())).Returns(true);

            var result = _wrapper.Equals("test");

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_DelegateToString_When_Called()
        {
            _mockReader.Setup(r => r.ToString()).Returns("SmartSqlDataReaderWrapper");

            var result = _wrapper.ToString();

            result.Should().Be("SmartSqlDataReaderWrapper");
        }

        [Fact]
        public void Should_DelegateGetProviderSpecificFieldType_When_Called()
        {
            _mockReader.Setup(r => r.GetProviderSpecificFieldType(0)).Returns(typeof(long));

            var result = _wrapper.GetProviderSpecificFieldType(0);

            result.Should().Be(typeof(long));
        }

        [Fact]
        public void Should_DelegateGetProviderSpecificValue_When_Called()
        {
            _mockReader.Setup(r => r.GetProviderSpecificValue(0)).Returns(42L);

            var result = _wrapper.GetProviderSpecificValue(0);

            result.Should().Be(42L);
        }

        [Fact]
        public void Should_DelegateGetProviderSpecificValues_When_Called()
        {
            var values = new object[2];
            _mockReader.Setup(r => r.GetProviderSpecificValues(It.IsAny<object[]>())).Returns(2);

            var result = _wrapper.GetProviderSpecificValues(values);

            result.Should().Be(2);
        }

        [Fact]
        public void Should_DelegateGetSchemaTable_When_Called()
        {
            var table = new System.Data.DataTable();
            _mockReader.Setup(r => r.GetSchemaTable()).Returns(table);

            var result = _wrapper.GetSchemaTable();

            result.Should().BeSameAs(table);
        }

        [Fact]
        public void Should_DelegateGetStream_When_Called()
        {
            var stream = new System.IO.MemoryStream();
            _mockReader.Setup(r => r.GetStream(0)).Returns(stream);

            var result = _wrapper.GetStream(0);

            result.Should().BeSameAs(stream);
        }

        [Fact]
        public void Should_DelegateGetTextReader_When_Called()
        {
            var reader = new System.IO.StringReader("test");
            _mockReader.Setup(r => r.GetTextReader(0)).Returns(reader);

            var result = _wrapper.GetTextReader(0);

            result.Should().BeSameAs(reader);
        }

        [Fact]
        public void Should_DelegateInitializeLifetimeService_When_Called()
        {
            // InitializeLifetimeService is from MarshalByRefObject and throws PlatformNotSupportedException on .NET 8
            var act = () => _wrapper.InitializeLifetimeService();

            act.Should().Throw<PlatformNotSupportedException>();
        }

        [Fact]
        public void Should_ReturnConstructorResult_When_SourceDataReaderSet()
        {
            new Mock<DbDataReader>().Setup(r => r.GetInt64(0)).Returns(123L);

            var mock = new Mock<DbDataReader>();
            mock.Setup(r => r.GetInt64(0)).Returns(123L);
            var wrapper = new DataReaderWrapper(mock.Object);

            wrapper.GetInt64(0).Should().Be(123L);
        }
    }
}
