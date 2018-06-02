using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.Abstractions.DataReaderDeserializer
{
    /// <summary>
    /// DataReader 反序列化器
    /// </summary>
    public interface IDataReaderDeserializer : IDataReaderDeserializerAsync
    {
        T ToSingle<T>(RequestContext context,IDataReader dataReader);
        IEnumerable<T> ToEnumerable<T>(RequestContext context, IDataReader dataReader);
    }
    public interface IDataReaderDeserializerAsync
    {
        Task<T> ToSingleAsync<T>(RequestContext context, IDataReader dataReader);
        Task<IEnumerable<T>> ToEnumerableAsync<T>(RequestContext context, IDataReader dataReader);
    }
}
