using SmartSql.Abstractions;
using SmartSql.Abstractions.DataReaderDeserializer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
namespace SmartSql.DapperDeserializer
{
    public class DapperDataReaderDeserializer : IDataReaderDeserializer
    {
        public IEnumerable<T> ToEnumerable<T>(RequestContext context, IDataReaderWrapper dataReader, bool isDispose = true)
        {
            try
            {
                IList<T> list = new List<T>();
                if (dataReader.Read())
                {
                    var targetType = typeof(T);
                    var rowParser = dataReader.GetRowParser(targetType);
                    do
                    {
                        var target = (T)rowParser(dataReader);
                        list.Add(target);
                    } while (dataReader.Read());
                }
                return list;
            }
            finally
            {
                Dispose(dataReader, isDispose);
            }
        }


        public T ToSingle<T>(RequestContext context, IDataReaderWrapper dataReader, bool isDispose = true)
        {
            try
            {
                if (dataReader.Read())
                {
                    var targetType = typeof(T);
                    var rowParser = dataReader.GetRowParser(targetType);
                    return (T)rowParser(dataReader);
                }
                return default(T);
            }
            finally
            {
                Dispose(dataReader, isDispose);
            }
        }
        #region Async
        public async Task<IEnumerable<T>> ToEnumerableAsync<T>(RequestContext context, IDataReaderWrapper dataReader, bool isDispose = true)
        {
            try
            {
                IList<T> list = new List<T>();
                if (await dataReader.ReadAsync())
                {
                    var targetType = typeof(T);
                    var rowParser = dataReader.GetRowParser(targetType);
                    do
                    {
                        var item = (T)rowParser(dataReader);
                        list.Add(item);
                    } while (await dataReader.ReadAsync());
                }
                return list;
            }
            finally
            {
                Dispose(dataReader, isDispose);
            }
        }

        public async Task<T> ToSingleAsync<T>(RequestContext context, IDataReaderWrapper dataReader, bool isDispose = true)
        {
            try
            {
                if (await dataReader.ReadAsync())
                {
                    var targetType = typeof(T);
                    var rowParser = dataReader.GetRowParser(targetType);
                    return (T)rowParser(dataReader);
                }
                return default(T);
            }
            finally
            {
                Dispose(dataReader, isDispose);
            }
        }

        #endregion
        private void Dispose(IDataReader dataReader, bool isDispose)
        {
            if (isDispose)
            {
                dataReader.Dispose();
                dataReader = null;
            }
        }

        public object ToSingle(RequestContext context, IDataReaderWrapper dataReader, Type targetType, bool isDispose = true)
        {
            try
            {
                dataReader.Read();
                var rowParser = dataReader.GetRowParser(targetType);
                return rowParser(dataReader);
            }
            finally
            {
                Dispose(dataReader, isDispose);
            }
        }

        public IEnumerable<object> ToEnumerable(RequestContext context, IDataReaderWrapper dataReader, Type targetType, bool isDispose = true)
        {
            try
            {
                IList<object> list = new List<object>();
                if (dataReader.Read())
                {
                    var rowParser = dataReader.GetRowParser(targetType);
                    do
                    {
                        var target = rowParser(dataReader);
                        list.Add(target);
                    } while (dataReader.Read());
                }
                return list;
            }
            finally
            {
                Dispose(dataReader, isDispose);
            }
        }

        public async Task<object> ToSingleAsync(RequestContext context, IDataReaderWrapper dataReader, Type targetType, bool isDispose = true)
        {
            try
            {
                await dataReader.ReadAsync();
                var rowParser = dataReader.GetRowParser(targetType);
                return rowParser(dataReader);
            }
            finally
            {
                Dispose(dataReader, isDispose);
            }
        }

        public async Task<IEnumerable<object>> ToEnumerableAsync(RequestContext context, IDataReaderWrapper dataReader, Type targetType, bool isDispose = true)
        {
            try
            {
                IList<object> list = new List<object>();
                if (await dataReader.ReadAsync())
                {
                    var rowParser = dataReader.GetRowParser(targetType);
                    do
                    {
                        var item = rowParser(dataReader);
                        list.Add(item);
                    } while (await dataReader.ReadAsync());
                }
                return list;
            }
            finally
            {
                Dispose(dataReader, isDispose);
            }
        }
    }
}
