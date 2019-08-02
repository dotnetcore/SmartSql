using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using static System.Char;

namespace SmartSql.SqlParsers
{
    public class CaseChangingCharStream : ICharStream
    {
        private readonly bool _upper;
        private readonly ICharStream _internalStream;

        public CaseChangingCharStream(ICharStream internalStream, bool upper)
        {
            _upper = upper;
            _internalStream = internalStream;
        }

        public int Index => _internalStream.Index;
        public int Size => _internalStream.Size;
        public string SourceName => _internalStream.SourceName;

        public void Consume()
        {
            _internalStream.Consume();
        }

        [return: NotNull]
        public string GetText(Interval interval) => _internalStream.GetText(interval);

        public int LA(int i)
        {
            var c = (char) _internalStream.LA(i);
            if (c <= 0)
            {
                return c;
            }

            return _upper ? ToUpperInvariant(c) : ToLowerInvariant(c);
        }

        public int Mark() => _internalStream.Mark();

        public void Release(int marker)
        {
            _internalStream.Release(marker);
        }

        public void Seek(int index)
        {
            _internalStream.Seek(index);
        }
    }
}