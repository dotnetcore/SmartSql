using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartSql.TypeHandlers;

namespace SmartSql.Deserializer
{
    public class ValueTypeDeserializer : IDataReaderDeserializer
    {
        public const int VALUE_ORDINAL = 0;
        public TResult ToSinge<TResult>(ExecutionContext executionContext)
        {
            var dataReader = executionContext.DataReaderWrapper;
            if (!dataReader.HasRows) return default(TResult);
            dataReader.Read();
            return TypeHandlerCache<TResult>.Handler.GetValue(dataReader, VALUE_ORDINAL); ;
        }
        public IList<TResult> ToList<TResult>(ExecutionContext executionContext)
        {
            var list = new List<TResult>();
            var dataReader = executionContext.DataReaderWrapper;
            if (!dataReader.HasRows) return list;

            var typeHandler = TypeHandlerCache<TResult>.Handler;
            while (dataReader.Read())
            {
                var val = typeHandler.GetValue(dataReader, VALUE_ORDINAL);
                list.Add(val);
            }
            return list;
        }

        public async Task<TResult> ToSingeAsync<TResult>(ExecutionContext executionContext)
        {
            var dataReader = executionContext.DataReaderWrapper;
            if (!dataReader.HasRows) return default(TResult);
            await dataReader.ReadAsync();
            return TypeHandlerCache<TResult>.Handler.GetValue(dataReader, VALUE_ORDINAL); 
        }

        public async Task<IList<TResult>> ToListAsync<TResult>(ExecutionContext executionContext)
        {
            var list = new List<TResult>();
            var dataReader = executionContext.DataReaderWrapper;
            if (!dataReader.HasRows) return list;
            var typeHandler = TypeHandlerCache<TResult>.Handler;
            while (await dataReader.ReadAsync())
            {
                var val = typeHandler.GetValue(dataReader, VALUE_ORDINAL);
                list.Add(val);
            }
            return list;
        }
    }
}
