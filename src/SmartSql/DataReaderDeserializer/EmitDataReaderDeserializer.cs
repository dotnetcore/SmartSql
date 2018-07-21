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
        public IEnumerable<T> ToEnumerable<T>(RequestContext context, IDataReader dataReader, bool isDispose = true)
        {
            try
            {
                Type targetType = typeof(T);
                if (dataReader.Read())
                {
                    var deser = _dataRowParserFactory.GetParser(dataReader, context, targetType);
                    do
                    {
                        object target = deser(dataReader, context);
                        yield return (T)target;
                    } while (dataReader.Read());
                }
            }
            finally
            {
               Dispose(dataReader, isDispose);
            }
        }

        private  void Dispose(IDataReader dataReader, bool isDispose)
        {
            if (isDispose)
            {
                dataReader.Dispose();
                dataReader = null;
            }
        }

        public async Task<IEnumerable<T>> ToEnumerableAsync<T>(RequestContext context, IDataReader dataReader, bool isDispose = true)
        {
            var dataReaderAsync = dataReader as DbDataReader;
            try
            {
                IList<T> list = new List<T>();
                Type targetType = typeof(T);

                if (await dataReaderAsync.ReadAsync())
                {
                    var deser = _dataRowParserFactory.GetParser(dataReader, context, targetType);
                    do
                    {
                        T target = (T)deser(dataReader, context);
                        list.Add(target);
                    } while (await dataReaderAsync.ReadAsync());
                }
                return list;
            }
            finally
            {
                Dispose(dataReader, isDispose);
            }
        }

        public T ToSingle<T>(RequestContext context, IDataReader dataReader, bool isDispose = true)
        {
            try
            {
                Type targetType = typeof(T);
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

        public async Task<T> ToSingleAsync<T>(RequestContext context, IDataReader dataReader, bool isDispose = true)
        {
            var dataReaderAsync = dataReader as DbDataReader;
            try
            {

                Type targetType = typeof(T);
                if (await dataReaderAsync.ReadAsync())
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
    }
}
