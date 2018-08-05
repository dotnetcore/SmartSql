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
        object ToSingle(RequestContext context, IDataReaderWrapper dataReader, Type targetType, bool isDispose = true);
        IEnumerable<object> ToEnumerable(RequestContext context, IDataReaderWrapper dataReader, Type targetType, bool isDispose = true);
        T ToSingle<T>(RequestContext context, IDataReaderWrapper dataReader, bool isDispose = true);
        IEnumerable<T> ToEnumerable<T>(RequestContext context, IDataReaderWrapper dataReader, bool isDispose = true);
    }
    public interface IDataReaderDeserializerAsync
    {
        Task<object> ToSingleAsync(RequestContext context, IDataReaderWrapper dataReader, Type targetType, bool isDispose = true);
        Task<IEnumerable<object>> ToEnumerableAsync(RequestContext context, IDataReaderWrapper dataReader, Type targetType , bool isDispose = true);

        Task<T> ToSingleAsync<T>(RequestContext context, IDataReaderWrapper dataReader, bool isDispose = true);
        Task<IEnumerable<T>> ToEnumerableAsync<T>(RequestContext context, IDataReaderWrapper dataReader, bool isDispose = true);
    }
}
