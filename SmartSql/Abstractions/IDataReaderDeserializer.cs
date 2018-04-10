using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.Abstractions
{
    /// <summary>
    /// DataReader 反序列化器
    /// </summary>
    public interface IDataReaderDeserializer : IDataReaderDeserializerAsync
    {
        object Deserialize(IDataReader dataReader, Type type);
        T Deserialize<T>(IDataReader dataReader);
    }

    public interface IDataReaderDeserializerAsync
    {
        Task<object> DeserializeAsync(DbDataReader dataReader, Type type);
        Task<T> DeserializeAsync<T>(DbDataReader dataReader);
    }
}
