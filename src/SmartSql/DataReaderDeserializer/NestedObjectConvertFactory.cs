using SmartSql.Abstractions;
using SmartSql.Abstractions.DataReaderDeserializer;
using SmartSql.Configuration.Maps;
using SmartSql.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;

namespace SmartSql.DataReaderDeserializer
{
    public class NestedObjectConvertFactory
    {
        private readonly IDictionary<string, Func<RequestContext, IDataReaderWrapper, IDataReaderDeserializer, object>> _cachedConvert = new Dictionary<string, Func<RequestContext, IDataReaderWrapper, IDataReaderDeserializer, object>>();

        private static readonly Type _enumerableType = typeof(IEnumerable);
        private static readonly Type _dataReaderType = typeof(IDataReader);
        private static readonly Type _dataReaderWrapperType = typeof(IDataReaderWrapper);
        private static readonly Type _deserType = typeof(IDataReaderDeserializer);
        private static readonly MethodInfo _deserToSingle = _deserType.GetMethod("ToSingle", new Type[] { TypeUtils.RequestContextType, _dataReaderWrapperType, TypeUtils.BooleanType });
        private static readonly MethodInfo _deserToEnumerable = _deserType.GetMethod("ToEnumerable", new Type[] { TypeUtils.RequestContextType, _dataReaderWrapperType, TypeUtils.BooleanType });
        private static readonly MethodInfo _dataReader_NextResult = _dataReaderType.GetMethod("NextResult");

        public Func<RequestContext, IDataReaderWrapper, IDataReaderDeserializer, object> CreateNestedObjectConvert(RequestContext requestContext, Type targetType)
        {
            string key = $"{requestContext.StatementKey}_{targetType.FullName}";
            if (!_cachedConvert.ContainsKey(key))
            {
                lock (this)
                {
                    if (!_cachedConvert.ContainsKey(key))
                    {
                        var convert = CreateNestedObjectConvertImpl(requestContext, targetType);
                        _cachedConvert.Add(key, convert);
                    }
                }
            }
            return _cachedConvert[key];
        }

        private Func<RequestContext, IDataReaderWrapper, IDataReaderDeserializer, object> CreateNestedObjectConvertImpl(RequestContext requestContext, Type targetType)
        {
            var dynamicMethod = new DynamicMethod("NestedObjectConvert_" + Guid.NewGuid().ToString("N"), targetType, new[] { TypeUtils.RequestContextType, _dataReaderWrapperType, _deserType }, targetType, true);
            var iLGenerator = dynamicMethod.GetILGenerator();
            iLGenerator.DeclareLocal(targetType);
            MultipleResultMap multipleResultMap = requestContext.MultipleResultMap;
            if (multipleResultMap.Root != null)
            {
                iLGenerator.Emit(OpCodes.Ldarg_2);
                iLGenerator.Emit(OpCodes.Ldarg_0);
                iLGenerator.Emit(OpCodes.Ldarg_1);
                iLGenerator.Emit(OpCodes.Ldc_I4_0);
                var deserToRoot = _deserToSingle.MakeGenericMethod(targetType);
                iLGenerator.Emit(OpCodes.Call, deserToRoot);
                iLGenerator.Emit(OpCodes.Stloc_0);
                EmitNextResult(iLGenerator);
            }
            else
            {
                ConstructorInfo targetCtor = targetType.GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
                iLGenerator.Emit(OpCodes.Newobj, targetCtor); // [target]
                iLGenerator.Emit(OpCodes.Stloc_0);
            }
            
            foreach (var result in multipleResultMap.Results)
            {
                var property = targetType.GetProperty(result.Property);
                var propertyType = property.PropertyType;
                bool isEnum = _enumerableType.IsAssignableFrom(propertyType);
                MethodInfo executeMethod = null;
                if (isEnum)
                {
                    var enumChildType = propertyType.GenericTypeArguments[0];
                    executeMethod = _deserToEnumerable.MakeGenericMethod(enumChildType);
                }
                else
                {
                    executeMethod = _deserToSingle.MakeGenericMethod(propertyType);
                }
                iLGenerator.Emit(OpCodes.Ldloc_0);
                iLGenerator.Emit(OpCodes.Ldarg_2);
                iLGenerator.Emit(OpCodes.Ldarg_0);
                iLGenerator.Emit(OpCodes.Ldarg_1);
                iLGenerator.Emit(OpCodes.Ldc_I4_0);
                iLGenerator.Emit(OpCodes.Call, executeMethod);
                iLGenerator.Emit(OpCodes.Call, property.SetMethod);
                EmitNextResult(iLGenerator);
            }
            iLGenerator.Emit(OpCodes.Ldloc_0);
            iLGenerator.Emit(OpCodes.Ret);
            var funcType = System.Linq.Expressions.Expression.GetFuncType(TypeUtils.RequestContextType, _dataReaderWrapperType, _deserType, targetType);
            return (Func<RequestContext, IDataReaderWrapper, IDataReaderDeserializer, object>)dynamicMethod.CreateDelegate(funcType);
        }

        private void EmitNextResult(ILGenerator iLGenerator)
        {
            iLGenerator.Emit(OpCodes.Ldarg_1);
            iLGenerator.Emit(OpCodes.Call, _dataReader_NextResult);
            iLGenerator.Emit(OpCodes.Pop);
        }
    }
}
