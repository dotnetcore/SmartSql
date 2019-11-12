using SmartSql.Configuration;
using SmartSql.Exceptions;
using SmartSql.Reflection.PropertyAccessor;
using SmartSql.Reflection.TypeConstants;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace SmartSql.Deserializer
{
    public class MultipleResultDeserializer : IDataReaderDeserializer
    {
        private readonly IDeserializerFactory _deserializerFactory;
        private readonly ISetAccessorFactory _setAccessorFactory;

        public MultipleResultDeserializer(IDeserializerFactory deserializerFactory)
        {
            _deserializerFactory = deserializerFactory;
            _setAccessorFactory = EmitSetAccessorFactory.Instance;
        }

        public bool CanDeserialize(ExecutionContext executionContext, Type resultType, bool isMultiple = false)
        {
            return isMultiple && !CommonType.IsValueTuple(resultType);
        }

        public TResult ToSingle<TResult>(ExecutionContext executionContext)
        {
            TResult result = default;
            var resultType = executionContext.Result.ResultType;
            var dataReader = executionContext.DataReaderWrapper;
            var multipleResultMap = executionContext.Request.MultipleResultMap;
            if (multipleResultMap.Root != null)
            {
                var deser = _deserializerFactory.Get(executionContext, executionContext.Result.ResultType);
                result = deser.ToSingle<TResult>(executionContext);
                if (result == null)
                {
                    return default(TResult);
                }

                dataReader.NextResult();
                
            }
            else
            {
                result = (TResult) executionContext.SmartSqlConfig.ObjectFactoryBuilder.GetObjectFactory(resultType,
                    Type.EmptyTypes)(null);
            }

            foreach (var resultMap in multipleResultMap.Results)
            {
                if (resultMap == multipleResultMap.Root)
                {
                    continue;
                }

                #region Set Muti Property

                var propertyInfo = resultType.GetProperty(resultMap.Property);
                var setProperty = _setAccessorFactory.Create(propertyInfo);
                var deser = _deserializerFactory.Get(executionContext, propertyInfo.PropertyType);
                var resultMapResult = TypeDeserializer.Deserialize(propertyInfo.PropertyType, deser, executionContext);
                setProperty(result, resultMapResult);

                #endregion

                if (!dataReader.NextResult())
                {
                    break;
                }
            }

            return result;
        }

        public async Task<TResult> ToSingleAsync<TResult>(ExecutionContext executionContext)
        {
            TResult result = default;
            var resultType = executionContext.Result.ResultType;
            var dataReader = executionContext.DataReaderWrapper;
            var multipleResultMap = executionContext.Request.MultipleResultMap;
            if (multipleResultMap.Root != null)
            {
                var deser = _deserializerFactory.Get(executionContext);
                result = deser.ToSingle<TResult>(executionContext);
                if (result == null)
                {
                    return default(TResult);
                }

                dataReader.NextResult();
            }
            else
            {
                result = (TResult) executionContext.SmartSqlConfig.ObjectFactoryBuilder.GetObjectFactory(resultType,
                    Type.EmptyTypes)(null);
            }

            foreach (var resultMap in multipleResultMap.Results)
            {
                #region Set Muti Property

                var propertyInfo = resultType.GetProperty(resultMap.Property);
                var setProperty = _setAccessorFactory.Create(propertyInfo);
                var deser = _deserializerFactory.Get(executionContext, propertyInfo.PropertyType);

                var resultMapResult = TypeDeserializer.Deserialize(propertyInfo.PropertyType, deser, executionContext);
                setProperty(result, resultMapResult);

                #endregion

                if (!await dataReader.NextResultAsync())
                {
                    break;
                }
            }

            return result;
        }

        public IList<TResult> ToList<TResult>(ExecutionContext executionContext)
        {
            throw new SmartSqlException("MultipleResultDeserializer can not support ToList.");
        }

        public Task<IList<TResult>> ToListAsync<TResult>(ExecutionContext executionContext)
        {
            throw new SmartSqlException("MultipleResultDeserializer can not support ToListAsync.");
        }
    }
}