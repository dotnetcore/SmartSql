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
    public class DataReaderWrapperTests
    {
        private readonly Mock<DbDataReader> _mockReader;
        private readonly DataReaderWrapper _wrapper;

        public DataReaderWrapperTests()
        {
            _mockReader = new Mock<DbDataReader>();
            _wrapper = new DataReaderWrapper(_mockReader.Object);
        }

        [Fact]
        public void Should_WrapDbDataReader_When_Created()
        {
            _wrapper.SourceDataReader.Should().BeSameAs(_mockReader.Object);
        }

        [Fact]
        public void Should_DelegateGetInt64_When_Called()
        {
            _mockReader.Setup(r => r.GetInt64(0)).Returns(42L);

            var result = _wrapper.GetInt64(0);

            result.Should().Be(42L);
            _mockReader.Verify(r => r.GetInt64(0), Times.Once);
        }

        [Fact]
        public void Should_DelegateGetString_When_Called()
        {
            _mockReader.Setup(r => r.GetString(1)).Returns("Hello");

            var result = _wrapper.GetString(1);

            result.Should().Be("Hello");
            _mockReader.Verify(r => r.GetString(1), Times.Once);
        }

        [Fact]
        public void Should_DelegateGetInt32_When_Called()
        {
            _mockReader.Setup(r => r.GetInt32(2)).Returns(100);

            var result = _wrapper.GetInt32(2);

            result.Should().Be(100);
        }

        [Fact]
        public void Should_IncrementResultIndex_When_NextResultCalled()
        {
            _mockReader.Setup(r => r.NextResult()).Returns(true);
            _wrapper.ResultIndex.Should().Be(0);

            _wrapper.NextResult();

            _wrapper.ResultIndex.Should().Be(1);

            _wrapper.NextResult();

            _wrapper.ResultIndex.Should().Be(2);
        }

        [Fact]
        public async Task Should_IncrementResultIndex_When_NextResultAsyncCalled()
        {
            _mockReader
                .Setup(r => r.NextResultAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await _wrapper.NextResultAsync(CancellationToken.None);

            _wrapper.ResultIndex.Should().Be(1);
        }

        [Fact]
        public void Should_ReturnFieldCount_When_Called()
        {
            _mockReader.Setup(r => r.FieldCount).Returns(5);

            _wrapper.FieldCount.Should().Be(5);
        }

        [Fact]
        public void Should_DelegateGetValue_When_Called()
        {
            _mockReader.Setup(r => r.GetValue(0)).Returns(42L);

            _wrapper.GetValue(0).Should().Be(42L);
        }

        [Fact]
        public void Should_DelegateGetName_When_Called()
        {
            _mockReader.Setup(r => r.GetName(0)).Returns("Id");

            _wrapper.GetName(0).Should().Be("Id");
        }

        [Fact]
        public void Should_DelegateGetFieldType_When_Called()
        {
            _mockReader.Setup(r => r.GetFieldType(0)).Returns(typeof(long));

            _wrapper.GetFieldType(0).Should().Be(typeof(long));
        }

        [Fact]
        public void Should_DelegateRead_When_Called()
        {
            _mockReader.Setup(r => r.Read()).Returns(true);

            _wrapper.Read().Should().BeTrue();
        }

        [Fact]
        public void Should_DelegateIsDBNull_When_Called()
        {
            _mockReader.Setup(r => r.IsDBNull(0)).Returns(true);

            _wrapper.IsDBNull(0).Should().BeTrue();
        }

        [Fact]
        public void Should_DelegateDepth_When_Called()
        {
            _mockReader.Setup(r => r.Depth).Returns(2);

            _wrapper.Depth.Should().Be(2);
        }

        [Fact]
        public void Should_DelegateHasRows_When_Called()
        {
            _mockReader.Setup(r => r.HasRows).Returns(true);

            _wrapper.HasRows.Should().BeTrue();
        }

        [Fact]
        public void Should_DelegateIsClosed_When_Called()
        {
            _mockReader.Setup(r => r.IsClosed).Returns(false);

            _wrapper.IsClosed.Should().BeFalse();
        }

        [Fact]
        public void Should_DelegateRecordsAffected_When_Called()
        {
            _mockReader.Setup(r => r.RecordsAffected).Returns(3);

            _wrapper.RecordsAffected.Should().Be(3);
        }

        [Fact]
        public void Should_DelegateIndexerByOrdinal_When_Called()
        {
            _mockReader.Setup(r => r[0]).Returns(42L);

            _wrapper[0].Should().Be(42L);
        }

        [Fact]
        public void Should_DelegateIndexerByName_When_Called()
        {
            _mockReader.Setup(r => r["Id"]).Returns(42L);

            _wrapper["Id"].Should().Be(42L);
        }
    }
}
