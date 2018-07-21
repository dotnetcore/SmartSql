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
        public IEnumerable<T> ToEnumerable<T>(RequestContext context, IDataReader dataReader, bool isDispose = true)
        {
            try
            {
                if (dataReader.Read())
                {
                    
                    var rowParser = dataReader.GetRowParser(typeof(T));
                    do
                    {
                        var target = (T)rowParser(dataReader);
                        yield return target;
                    } while (dataReader.Read());
                }
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
                dataReader.Read();
                var rowParser = Dapper.SqlMapper.GetRowParser(dataReader, typeof(T));
                return (T)rowParser(dataReader);
            }
            finally
            {
                Dispose(dataReader, isDispose);
            }
        }
        #region Async
        public async Task<IEnumerable<T>> ToEnumerableAsync<T>(RequestContext context, IDataReader dataReader, bool isDispose = true)
        {
            try
            {
                IList<T> list = new List<T>();
                var dataReaderAsync = dataReader as DbDataReader;
                if (await dataReaderAsync.ReadAsync())
                {
                    var rowParser = Dapper.SqlMapper.GetRowParser(dataReader, typeof(T));
                    do
                    {
                        var item = (T)rowParser(dataReader);
                        list.Add(item);
                    } while (await dataReaderAsync.ReadAsync());
                }
                return list;
            }
            finally
            {
                Dispose(dataReader, isDispose);
            }
        }

        public async Task<T> ToSingleAsync<T>(RequestContext context, IDataReader dataReader, bool isDispose = true)
        {
            try
            {
                var dataReaderAsync = dataReader as DbDataReader;
                await dataReaderAsync.ReadAsync();
                var rowParser = Dapper.SqlMapper.GetRowParser(dataReader, typeof(T));
                return (T)rowParser(dataReader);
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
    }
}
