using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartSql.Reflection.TypeConstants;
using SmartSql.TypeHandlers;

namespace SmartSql.Deserializer
{
    public class ValueTypeDeserializer : IDataReaderDeserializer
    {
        public const int VALUE_ORDINAL = 0;
        public bool CanDeserialize(ExecutionContext executionContext, Type resultType, bool isMultiple = false)
        {
            return resultType.IsValueType || resultType == CommonType.String;
        }

        public TResult ToSingle<TResult>(ExecutionContext executionContext)
        {
            var dataReader = executionContext.DataReaderWrapper;
            if (!dataReader.HasRows) return default(TResult);
            dataReader.Read();
            return TypeHandlerCache<TResult, AnyFieldType>.Handler.GetValue(dataReader, VALUE_ORDINAL, executionContext.Result.ResultType); ;
        }
        public IList<TResult> ToList<TResult>(ExecutionContext executionContext)
        {
            var list = new List<TResult>();
            var dataReader = executionContext.DataReaderWrapper;
            if (!dataReader.HasRows) return list;

            var typeHandler = TypeHandlerCache<TResult, AnyFieldType>.Handler;
            while (dataReader.Read())
            {
                var val = typeHandler.GetValue(dataReader, VALUE_ORDINAL, executionContext.Result.ResultType);
                list.Add(val);
            }
            return list;
        }

        public async Task<TResult> ToSingleAsync<TResult>(ExecutionContext executionContext)
        {
            var dataReader = executionContext.DataReaderWrapper;
            if (!dataReader.HasRows) return default(TResult);
            await dataReader.ReadAsync();
            return TypeHandlerCache<TResult, AnyFieldType>.Handler.GetValue(dataReader, VALUE_ORDINAL, executionContext.Result.ResultType);
        }

        public async Task<IList<TResult>> ToListAsync<TResult>(ExecutionContext executionContext)
        {
            var list = new List<TResult>();
            var dataReader = executionContext.DataReaderWrapper;
            if (!dataReader.HasRows) return list;
            var typeHandler = TypeHandlerCache<TResult, AnyFieldType>.Handler;
            while (await dataReader.ReadAsync())
            {
                var val = typeHandler.GetValue(dataReader, VALUE_ORDINAL, executionContext.Result.ResultType);
                list.Add(val);
            }
            return list;
        }
    }
}
