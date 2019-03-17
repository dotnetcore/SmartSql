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
        private readonly IDictionary<Result, Func<IDataReaderDeserializer, ExecutionContext, object>> _cachedGetResult = new Dictionary<Result, Func<IDataReaderDeserializer, ExecutionContext, object>>();

        private readonly IDeserializerFactory _deserializerFactory;
        private readonly ISetAccessorFactory _setAccessorFactory;
        public MultipleResultDeserializer(IDeserializerFactory deserializerFactory)
        {
            _deserializerFactory = deserializerFactory;
            _setAccessorFactory = new EmitSetAccessorFactory();
        }
        public TResult ToSinge<TResult>(ExecutionContext executionContext)
        {
            TResult result = default;
            var resultType = executionContext.Result.ResultType;
            var dataReader = executionContext.DataReaderWrapper;
            var multipleResultMap = executionContext.Request.MultipleResultMap;
            if (multipleResultMap.Root != null)
            {
                var deser = _deserializerFactory.Get(executionContext.Result.ResultType);
                result = deser.ToSinge<TResult>(executionContext);
                if (result == null)
                {
                    return default(TResult);
                }
                dataReader.NextResult();
            }
            else
            {
                result = (TResult)executionContext.SmartSqlConfig.ObjectFactoryBuilder.GetObjectFactory(resultType, Type.EmptyTypes)(null);
            }
            foreach (var resultMap in multipleResultMap.Results)
            {
                #region Set Muti Property
                var propertyInfo = resultType.GetProperty(resultMap.Property);
                var setProperty = _setAccessorFactory.Create(propertyInfo);
                var deser = _deserializerFactory.Get(propertyInfo.PropertyType);
                var resultMapResult = GetGetResult(resultMap, propertyInfo)(deser, executionContext);
                setProperty(result, resultMapResult);
                #endregion
                if (!dataReader.NextResult())
                {
                    break;
                }
            }
            return result;
        }

        public async Task<TResult> ToSingeAsync<TResult>(ExecutionContext executionContext)
        {
            TResult result = default;
            var resultType = executionContext.Result.ResultType;
            var dataReader = executionContext.DataReaderWrapper;
            var multipleResultMap = executionContext.Request.MultipleResultMap;
            if (multipleResultMap.Root != null)
            {
                var deser = _deserializerFactory.Get(executionContext.Result.ResultType);
                result = deser.ToSinge<TResult>(executionContext);
                if (result == null)
                {
                    return default(TResult);
                }
                dataReader.NextResult();
            }
            else
            {
                result = (TResult)executionContext.SmartSqlConfig.ObjectFactoryBuilder.GetObjectFactory(resultType, Type.EmptyTypes)(null);
            }
            foreach (var resultMap in multipleResultMap.Results)
            {
                #region Set Muti Property
                var propertyInfo = resultType.GetProperty(resultMap.Property);
                var setProperty = _setAccessorFactory.Create(propertyInfo);
                var deser = _deserializerFactory.Get(propertyInfo.PropertyType);
                var resultMapResult = GetGetResult(resultMap, propertyInfo)(deser, executionContext);
                setProperty(result, resultMapResult);
                #endregion
                if (!await dataReader.NextResultAsync())
                {
                    break;
                }
            }
            return result;
        }

        private Func<IDataReaderDeserializer, ExecutionContext, object> GetGetResult(Result resultMap, PropertyInfo propertyInfo)
        {
            if (!_cachedGetResult.ContainsKey(resultMap))
            {
                lock (this)
                {
                    if (!_cachedGetResult.ContainsKey(resultMap))
                    {
                        var impl = CreateGetResult(propertyInfo);
                        _cachedGetResult.Add(resultMap, impl);
                    }
                }
            }
            return _cachedGetResult[resultMap];
        }
        private Func<IDataReaderDeserializer, ExecutionContext, object> CreateGetResult(PropertyInfo propertyInfo)
        {
            var dynamicMethod = new DynamicMethod("CreateGetResult_" + Guid.NewGuid().ToString("N"), CommonType.Object, new[] { IDataReaderDeserializerType.Type, ExecutionContextType.Type });
            var ilGen = dynamicMethod.GetILGenerator();
            ilGen.LoadArg(0);
            ilGen.LoadArg(1);
            MethodInfo deserMethod;
            if (CommonType.IEnumerable.IsAssignableFrom(propertyInfo.PropertyType))
            {
                var listItemType = propertyInfo.PropertyType.GenericTypeArguments[0];
                deserMethod = IDataReaderDeserializerType.Method.MakeGenericToList(listItemType);
            }
            else
            {
                deserMethod = IDataReaderDeserializerType.Method.MakeGenericToSinge(propertyInfo.PropertyType);
            }
            ilGen.Call(deserMethod);
            if (propertyInfo.PropertyType.IsValueType)
            {
                ilGen.Box(propertyInfo.PropertyType);
            }
            ilGen.Return();
            return (Func<IDataReaderDeserializer, ExecutionContext, object>)dynamicMethod.CreateDelegate(typeof(Func<IDataReaderDeserializer, ExecutionContext, object>));
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
