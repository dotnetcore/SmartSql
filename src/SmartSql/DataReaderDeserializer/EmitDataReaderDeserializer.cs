using SmartSql.Abstractions;
using SmartSql.Abstractions.DataReaderDeserializer;
using SmartSql.Abstractions.TypeHandler;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace SmartSql.DataReaderDeserializer
{
    public class EmitDataReaderDeserializer : IDataReaderDeserializer
    {
        DataRowParserFactory _dataRowParserFactory;
        public EmitDataReaderDeserializer()
        {
            _dataRowParserFactory = new DataRowParserFactory();
        }
        public IEnumerable<T> ToEnumerable<T>(RequestContext context, IDataReaderWrapper dataReader, bool isDispose = true)
        {
            try
            {
                IList<T> list = new List<T>();
                var targetType = typeof(T);
                if (dataReader.Read())
                {
                    var deser = _dataRowParserFactory.GetParser(dataReader, context, targetType);
                    do
                    {
                        T target = (T)deser(dataReader, context);
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
        public IEnumerable<object> ToEnumerable(RequestContext context, IDataReaderWrapper dataReader, Type targetType, bool isDispose = true)
        {
            try
            {
                IList<object> list = new List<object>();
                if (dataReader.Read())
                {
                    var deser = _dataRowParserFactory.GetParser(dataReader, context, targetType);
                    do
                    {
                        var target = deser(dataReader, context);
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

        private void Dispose(IDataReader dataReader, bool isDispose)
        {
            if (isDispose)
            {
                dataReader.Dispose();
                dataReader = null;
            }
        }

        public async Task<IEnumerable<T>> ToEnumerableAsync<T>(RequestContext context, IDataReaderWrapper dataReader, bool isDispose = true)
        {
            try
            {
                IList<T> list = new List<T>();
                var targetType = typeof(T);
                if (await dataReader.ReadAsync())
                {
                    var deser = _dataRowParserFactory.GetParser(dataReader, context, targetType);
                    do
                    {
                        T target = (T)deser(dataReader, context);
                        list.Add(target);
                    } while (await dataReader.ReadAsync());
                }
                return list;
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
                    var deser = _dataRowParserFactory.GetParser(dataReader, context, targetType);
                    do
                    {
                        var target = deser(dataReader, context);
                        list.Add(target);
                    } while (await dataReader.ReadAsync());
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
                var targetType = typeof(T);
                if (dataReader.Read())
                {
                    var deser = _dataRowParserFactory.GetParser(dataReader, context, targetType);
                    object target = deser(dataReader, context);
                    return (T)target;
                }
                return default(T);
            }
            finally
            {
                Dispose(dataReader, isDispose);
            }
        }

        public object ToSingle(RequestContext context, IDataReaderWrapper dataReader, Type targetType, bool isDispose = true)
        {
            try
            {
                if (dataReader.Read())
                {
                    var deser = _dataRowParserFactory.GetParser(dataReader, context, targetType);
                    return deser(dataReader, context);
                }
                return null;

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
                var targetType = typeof(T);
                if (await dataReader.ReadAsync())
                {
                    var deser = _dataRowParserFactory.GetParser(dataReader, context, targetType);
                    object target = deser(dataReader, context);
                    return (T)target;
                }
                return default(T);
            }
            finally
            {
                Dispose(dataReader, isDispose);
            }
        }
        public async Task<object> ToSingleAsync(RequestContext context, IDataReaderWrapper dataReader, Type targetType = null, bool isDispose = true)
        {
            try
            {
                await dataReader.ReadAsync();
                var deser = _dataRowParserFactory.GetParser(dataReader, context, targetType);
                return deser(dataReader, context);
            }
            finally
            {
                Dispose(dataReader, isDispose);
            }
        }
    }
}
