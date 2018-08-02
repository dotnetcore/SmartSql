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
        T ToSingle<T>(RequestContext context, IDataReaderWrapper dataReader, bool isDispose = true);
        IEnumerable<T> ToEnumerable<T>(RequestContext context, IDataReaderWrapper dataReader, bool isDispose = true);
    }
    public interface IDataReaderDeserializerAsync
    {
        Task<T> ToSingleAsync<T>(RequestContext context, IDataReaderWrapper dataReader, bool isDispose = true);
        Task<IEnumerable<T>> ToEnumerableAsync<T>(RequestContext context, IDataReaderWrapper dataReader, bool isDispose = true);
    }
}
