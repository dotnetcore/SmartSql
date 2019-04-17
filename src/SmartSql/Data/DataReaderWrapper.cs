using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartSql.Data
{
    public class DataReaderWrapper : DbDataReader
    {
        public DbDataReader SourceDataReader { get; }

        public override object this[int ordinal] => SourceDataReader[ordinal];

        public override object this[string name] => SourceDataReader[name];

        public override int Depth => SourceDataReader.Depth;

        public override int FieldCount => SourceDataReader.FieldCount;

        public override bool HasRows => SourceDataReader.HasRows;

        public override bool IsClosed => SourceDataReader.IsClosed;

        public override int RecordsAffected => SourceDataReader.RecordsAffected;

        public int ResultIndex { get; private set; }

        public DataReaderWrapper(DbDataReader dbDataReader)
        {
            SourceDataReader = dbDataReader;
        }

        public override bool NextResult()
        {
            ResultIndex++;
            return SourceDataReader.NextResult();
        }

        public override Task<bool> NextResultAsync(CancellationToken cancellationToken)
        {
            ResultIndex++;
            return SourceDataReader.NextResultAsync(cancellationToken);
        }

        public override bool GetBoolean(int ordinal) => SourceDataReader.GetBoolean(ordinal);

        public override byte GetByte(int ordinal) => SourceDataReader.GetByte(ordinal);

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
            => SourceDataReader.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);

        public override char GetChar(int ordinal) => SourceDataReader.GetChar(ordinal);

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        => SourceDataReader.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);

        public override string GetDataTypeName(int ordinal) => SourceDataReader.GetDataTypeName(ordinal);

        public override DateTime GetDateTime(int ordinal) => SourceDataReader.GetDateTime(ordinal);

        public override decimal GetDecimal(int ordinal) => SourceDataReader.GetDecimal(ordinal);

        public override double GetDouble(int ordinal) => SourceDataReader.GetDouble(ordinal);

        public override IEnumerator GetEnumerator() => SourceDataReader.GetEnumerator();

        public override Type GetFieldType(int ordinal) => SourceDataReader.GetFieldType(ordinal);

        public override float GetFloat(int ordinal) => SourceDataReader.GetFloat(ordinal);

        public override Guid GetGuid(int ordinal) => SourceDataReader.GetGuid(ordinal);

        public override short GetInt16(int ordinal) => SourceDataReader.GetInt16(ordinal);

        public override int GetInt32(int ordinal) => SourceDataReader.GetInt32(ordinal);

        public override long GetInt64(int ordinal) => SourceDataReader.GetInt64(ordinal);

        public override string GetName(int ordinal) => SourceDataReader.GetName(ordinal);

        public override int GetOrdinal(string name) => SourceDataReader.GetOrdinal(name);

        public override string GetString(int ordinal) => SourceDataReader.GetString(ordinal);

        public override object GetValue(int ordinal) => SourceDataReader.GetValue(ordinal);

        public override int GetValues(object[] values) => SourceDataReader.GetValues(values);

        public override bool IsDBNull(int ordinal) => SourceDataReader.IsDBNull(ordinal);

        public override bool Read() => SourceDataReader.Read();
        public override void Close() => SourceDataReader.Close();
        public override bool Equals(object obj) => SourceDataReader.Equals(obj);
        public override T GetFieldValue<T>(int ordinal) => SourceDataReader.GetFieldValue<T>(ordinal);
        public override Task<T> GetFieldValueAsync<T>(int ordinal, CancellationToken cancellationToken) => SourceDataReader.GetFieldValueAsync<T>(ordinal, cancellationToken);
        public override int GetHashCode() => SourceDataReader.GetHashCode();
        public override Type GetProviderSpecificFieldType(int ordinal) => SourceDataReader.GetProviderSpecificFieldType(ordinal);
        public override object GetProviderSpecificValue(int ordinal) => SourceDataReader.GetProviderSpecificValue(ordinal);
        public override int GetProviderSpecificValues(object[] values) => SourceDataReader.GetProviderSpecificValues(values);
        public override DataTable GetSchemaTable() => SourceDataReader.GetSchemaTable();
        public override Stream GetStream(int ordinal) => SourceDataReader.GetStream(ordinal);
        public override TextReader GetTextReader(int ordinal) => SourceDataReader.GetTextReader(ordinal);
        public override object InitializeLifetimeService() => SourceDataReader.InitializeLifetimeService();
        public override Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken) => SourceDataReader.IsDBNullAsync(ordinal, cancellationToken);
        public override Task<bool> ReadAsync(CancellationToken cancellationToken) => SourceDataReader.ReadAsync(cancellationToken);
        public override string ToString() => SourceDataReader.ToString();
    }
}
