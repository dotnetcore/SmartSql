using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartSql.Exceptions;
using SmartSql.Reflection;
using SmartSql.Reflection.TypeConstants;

namespace SmartSql.Deserializer
{
    public class ValueTupleDeserializer : IDataReaderDeserializer
    {
        private readonly IDeserializerFactory _deserializerFactory;

        public ValueTupleDeserializer(IDeserializerFactory deserializerFactory)
        {
            _deserializerFactory = deserializerFactory;
        }

        public bool CanDeserialize(ExecutionContext executionContext, Type resultType, bool isMultiple = false)
        {
            return CommonType.IsValueTuple(resultType);
        }

        public TResult ToSingle<TResult>(ExecutionContext executionContext)
        {
            var valueTupleType = typeof(TResult);
            var resultGenericTypeArguments = valueTupleType.GenericTypeArguments;
            var resultItems = new object[resultGenericTypeArguments.Length];
            for (int i = 0; i < resultGenericTypeArguments.Length; i++)
            {
                var argType = resultGenericTypeArguments[i];
                var deser = _deserializerFactory.Get(executionContext, argType);
                resultItems[i] = TypeDeserializer.Deserialize(argType, deser, executionContext);
                if (!executionContext.DataReaderWrapper.NextResult())
                {
                    break;
                }
            }
            return (TResult)ValueTupleConvert.Convert(valueTupleType, resultItems);
        }
        public async Task<TResult> ToSingleAsync<TResult>(ExecutionContext executionContext)
        {
            var valueTupleType = typeof(TResult);
            var resultGenericTypeArguments = valueTupleType.GenericTypeArguments;
            var resultItems = new object[resultGenericTypeArguments.Length];
            for (int i = 0; i < resultGenericTypeArguments.Length; i++)
            {
                var argType = resultGenericTypeArguments[i];
                var deser = _deserializerFactory.Get(executionContext, argType);
                resultItems[i] = TypeDeserializer.Deserialize(argType, deser, executionContext);
                if (!await executionContext.DataReaderWrapper.NextResultAsync())
                {
                    break;
                }
            }
            return (TResult)ValueTupleConvert.Convert(valueTupleType, resultItems);
        }

        public Task<IList<TResult>> ToListAsync<TResult>(ExecutionContext executionContext)
        {
            throw new SmartSqlException("MultipleResultDeserializer can not support ToListAsync.");
        }


        public IList<TResult> ToList<TResult>(ExecutionContext executionContext)
        {
            throw new SmartSqlException("MultipleResultDeserializer can not support ToList.");
        }
    }
}
