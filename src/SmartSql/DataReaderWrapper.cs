using SmartSql.Abstractions.DataReaderDeserializer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using SmartSql.Abstractions;

namespace SmartSql
{
    public class DataReaderWrapper : IDataReaderWrapper
    {
        private readonly IDataReader _dataReader;

        public DataReaderWrapper(IDataReader dataReader)
        {
            _dataReader = dataReader;
        }
        public int ResultIndex { get; private set; }
        #region IDataReader Impl
        public object this[int i] => _dataReader[i];
        public object this[string name] => _dataReader[name];
        public int Depth => _dataReader.Depth;

        public bool IsClosed => _dataReader.IsClosed;

        public int RecordsAffected => _dataReader.RecordsAffected;

        public int FieldCount => _dataReader.FieldCount;

        public void Close()
        {
            _dataReader.Close();
        }

        public void Dispose()
        {
            _dataReader.Dispose();
        }

        public bool GetBoolean(int i)
        {
            return _dataReader.GetBoolean(i);
        }

        public byte GetByte(int i)
        {
            return _dataReader.GetByte(i);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return _dataReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        public char GetChar(int i)
        {
            return _dataReader.GetChar(i);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return _dataReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        public IDataReader GetData(int i)
        {
            return _dataReader.GetData(i);
        }

        public string GetDataTypeName(int i)
        {
            return _dataReader.GetDataTypeName(i);
        }

        public DateTime GetDateTime(int i)
        {
            return _dataReader.GetDateTime(i);
        }

        public decimal GetDecimal(int i)
        {
            return _dataReader.GetDecimal(i);
        }

        public double GetDouble(int i)
        {
            return _dataReader.GetDouble(i);
        }

        public Type GetFieldType(int i)
        {
            return _dataReader.GetFieldType(i);
        }

        public float GetFloat(int i)
        {
            return _dataReader.GetFloat(i);
        }

        public Guid GetGuid(int i)
        {
            return _dataReader.GetGuid(i);
        }

        public short GetInt16(int i)
        {
            return _dataReader.GetInt16(i);
        }

        public int GetInt32(int i)
        {
            return _dataReader.GetInt32(i);
        }

        public long GetInt64(int i)
        {
            return _dataReader.GetInt64(i);
        }

        public string GetName(int i)
        {
            return _dataReader.GetName(i);
        }

        public int GetOrdinal(string name)
        {
            return _dataReader.GetOrdinal(name);
        }

        public DataTable GetSchemaTable()
        {
            return _dataReader.GetSchemaTable();
        }

        public string GetString(int i)
        {
            return _dataReader.GetString(i);
        }

        public object GetValue(int i)
        {
            return _dataReader.GetValue(i);
        }

        public int GetValues(object[] values)
        {
            return _dataReader.GetValues(values);
        }

        public bool IsDBNull(int i)
        {
            return _dataReader.IsDBNull(i);
        }

        public bool NextResult()
        {
            var result = _dataReader.NextResult();
            ResultIndex++;
            return result;
        }

        public async Task<bool> NextResultAsync()
        {
            var dataReaderAsync = _dataReader as DbDataReader;
            var result = await dataReaderAsync.NextResultAsync();
            ResultIndex++;
            return result;
        }

        public bool Read()
        {
            return _dataReader.Read();
        }
        public Task<bool> ReadAsync()
        {
            var dataReaderAsync = _dataReader as DbDataReader;
            return dataReaderAsync.ReadAsync();
        }

        #endregion
    }
}
