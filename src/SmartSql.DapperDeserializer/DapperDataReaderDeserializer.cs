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

namespace SmartSql.DapperDeserializer
{
    public class DapperDataReaderDeserializer : IDataReaderDeserializer
    {
        public IEnumerable<T> ToEnumerable<T>(RequestContext context, IDataReader dataReader)
        {
            try
            {
                if (dataReader.Read())
                {
                    var rowParser = Dapper.SqlMapper.GetRowParser(dataReader, typeof(T));
                    do
                    {
                        var target = (T)rowParser(dataReader);
                        yield return target;
                    } while (dataReader.Read());
                }
            }
            finally
            {
                dataReader.Dispose();
                dataReader.Close();
            }
        }


        public T ToSingle<T>(RequestContext context, IDataReader dataReader)
        {
            try
            {
                dataReader.Read();
                var rowParser = Dapper.SqlMapper.GetRowParser(dataReader, typeof(T));
                return (T)rowParser(dataReader);
            }
            finally
            {
                dataReader.Dispose();
                dataReader.Close();
            }
        }
        #region Async
        public async Task<IEnumerable<T>> ToEnumerableAsync<T>(RequestContext context, IDataReader dataReader)
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
                dataReader.Dispose();
                dataReader.Close();
            }
        }

        public async Task<T> ToSingleAsync<T>(RequestContext context, IDataReader dataReader)
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
                dataReader.Dispose();
                dataReader.Close();
            }
        }

        #endregion

    }
}
