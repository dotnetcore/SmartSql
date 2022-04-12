using System;
using System.Collections;
using System.Data.Common;
using SmartSql.Data;

namespace SmartSql.Test.Unit.TypeHandlers;

public class MockTypeHandlerDbDataReader : DbDataReader
{
    private readonly object _value;

    public MockTypeHandlerDbDataReader(object value)
    {
        _value = value;
    }

    public static DataReaderWrapper Of(object value)
    {
        MockTypeHandlerDbDataReader dataReader = new MockTypeHandlerDbDataReader(value);
        return new DataReaderWrapper(dataReader);
    }

    public override bool GetBoolean(int ordinal)
    {
        return (bool)_value;
    }

    public override byte GetByte(int ordinal)
    {
        return (byte)_value;
    }

    public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length)
    {
        throw new NotImplementedException();
    }

    public override char GetChar(int ordinal)
    {
        return (char)_value;
    }

    public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length)
    {
        throw new NotImplementedException();
    }

    public override string GetDataTypeName(int ordinal)
    {
        throw new NotImplementedException();
    }

    public override DateTime GetDateTime(int ordinal)
    {
        return (DateTime)_value;
    }

    public override decimal GetDecimal(int ordinal)
    {
        return (decimal)_value;
    }

    public override double GetDouble(int ordinal)
    {
        return (double)_value;
    }

    public override Type GetFieldType(int ordinal)
    {
        throw new NotImplementedException();
    }

    public override float GetFloat(int ordinal)
    {
        return (float)_value;
    }

    public override Guid GetGuid(int ordinal)
    {
        return (Guid)_value;
    }

    public override short GetInt16(int ordinal)
    {
        return (short)_value;
    }

    public override int GetInt32(int ordinal)
    {
        return (int)_value;
    }

    public override long GetInt64(int ordinal)
    {
        return (long)_value;
    }

    public override string GetName(int ordinal)
    {
        throw new NotImplementedException();
    }

    public override int GetOrdinal(string name)
    {
        throw new NotImplementedException();
    }

    public override string GetString(int ordinal)
    {
        return (string)_value;
    }

    public override object GetValue(int ordinal)
    {
        return _value;
    }

    public override int GetValues(object[] values)
    {
        throw new NotImplementedException();
    }

    public override bool IsDBNull(int ordinal)
    {
        throw new NotImplementedException();
    }

    public override int FieldCount { get; }

    public override object this[int ordinal] => throw new NotImplementedException();

    public override object this[string name] => throw new NotImplementedException();

    public override int RecordsAffected { get; }
    public override bool HasRows { get; }
    public override bool IsClosed { get; }

    public override bool NextResult()
    {
        throw new NotImplementedException();
    }

    public override bool Read()
    {
        throw new NotImplementedException();
    }

    public override int Depth { get; }

    public override IEnumerator GetEnumerator()
    {
        throw new NotImplementedException();
    }
}