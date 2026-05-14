using System;
using System.Collections;
using System.Data.Common;
using SmartSql.Data;

namespace SmartSql.Test.Unit.TypeHandlers
{
    public class EnhancedMockTypeHandlerDbDataReader : DbDataReader
    {
        private readonly object _value;
        private readonly bool _isDbNull;
        private readonly string _columnName;

        public EnhancedMockTypeHandlerDbDataReader(object value, bool isDbNull = false, string columnName = "TestColumn")
        {
            _value = value;
            _isDbNull = isDbNull;
            _columnName = columnName;
        }

        public static DataReaderWrapper Of(object value, string columnName = "TestColumn")
        {
            var dataReader = new EnhancedMockTypeHandlerDbDataReader(value, false, columnName);
            return new DataReaderWrapper(dataReader);
        }

        public static DataReaderWrapper OfNull()
        {
            var dataReader = new EnhancedMockTypeHandlerDbDataReader(null, true);
            return new DataReaderWrapper(dataReader);
        }

        public override bool GetBoolean(int ordinal) => (bool)_value;
        public override byte GetByte(int ordinal) => (byte)_value;
        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
            => throw new NotImplementedException();
        public override char GetChar(int ordinal) => (char)_value;
        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
            => throw new NotImplementedException();
        public override string GetDataTypeName(int ordinal) => throw new NotImplementedException();
        public override DateTime GetDateTime(int ordinal) => (DateTime)_value;
        public override decimal GetDecimal(int ordinal) => (decimal)_value;
        public override double GetDouble(int ordinal) => (double)_value;
        public override Type GetFieldType(int ordinal) => throw new NotImplementedException();
        public override float GetFloat(int ordinal) => (float)_value;
        public override Guid GetGuid(int ordinal) => (Guid)_value;
        public override short GetInt16(int ordinal) => (short)_value;
        public override int GetInt32(int ordinal) => (int)_value;
        public override long GetInt64(int ordinal) => (long)_value;
        public override string GetName(int ordinal) => _columnName;
        public override int GetOrdinal(string name) => throw new NotImplementedException();
        public override string GetString(int ordinal) => (string)_value;
        public override object GetValue(int ordinal) => _value;
        public override int GetValues(object[] values) => throw new NotImplementedException();
        public override bool IsDBNull(int ordinal) => _isDbNull;
        public override int FieldCount { get; }
        public override object this[int ordinal] => throw new NotImplementedException();
        public override object this[string name] => throw new NotImplementedException();
        public override int RecordsAffected { get; }
        public override bool HasRows { get; }
        public override bool IsClosed { get; }
        public override bool NextResult() => throw new NotImplementedException();
        public override bool Read() => throw new NotImplementedException();
        public override int Depth { get; }
        public override IEnumerator GetEnumerator() => throw new NotImplementedException();
    }
}
