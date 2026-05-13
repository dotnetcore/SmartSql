using SmartSql.TypeHandlers;
using Xunit;

namespace SmartSql.Test.Unit.TypeHandlers
{
    public class Int64TypeHandlerTest
    {
        [Fact]
        public void Int64ToInt64()
        {
            var typeHandler = new Int64TypeHandler();
            long expected = 1L;
            var actual = typeHandler.GetValue(MockTypeHandlerDbDataReader.Of(expected), 1, typeof(long));
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void ByteToInt64()
        {
            var typeHandler = new Int64ByteTypeHandler();
            byte expected = 1;
            var actual = typeHandler.GetValue(MockTypeHandlerDbDataReader.Of(expected), 1, typeof(byte));
            Assert.Equal(expected, actual);
        }
    }
}