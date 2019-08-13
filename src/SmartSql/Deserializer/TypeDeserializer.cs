using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using SmartSql.Configuration;
using SmartSql.Reflection.TypeConstants;
using SmartSql.Utils;

namespace SmartSql.Deserializer
{
    public class TypeDeserializer
    {
        public static object Deserialize(Type resultType, IDataReaderDeserializer deserializer, ExecutionContext executionContext)
        {
            return GetImpl(resultType)(deserializer, executionContext);
        }

        private static Func<IDataReaderDeserializer, ExecutionContext, object> GetImpl(Type resultType)
        {
            return CacheUtil<TypeDeserializer, Type, Func<IDataReaderDeserializer, ExecutionContext, object>>
                  .GetOrAdd(resultType, CreateImpl);
        }

        private static Func<IDataReaderDeserializer, ExecutionContext, object> CreateImpl(Type resultType)
        {
            var dynamicMethod = new DynamicMethod("CreateGetResult_" + Guid.NewGuid().ToString("N"), CommonType.Object, new[] { IDataReaderDeserializerType.Type, ExecutionContextType.Type });
            var ilGen = dynamicMethod.GetILGenerator();
            ilGen.LoadArg(0);
            ilGen.LoadArg(1);
            MethodInfo deserMethod;
            if (CommonType.IEnumerable.IsAssignableFrom(resultType))
            {
                var listItemType = resultType.GenericTypeArguments[0];
                deserMethod = IDataReaderDeserializerType.Method.MakeGenericToList(listItemType);
            }
            else
            {
                deserMethod = IDataReaderDeserializerType.Method.MakeGenericToSingle(resultType);
            }
            ilGen.Callvirt(deserMethod);
            if (resultType.IsValueType)
            {
                ilGen.Box(resultType);
            }
            ilGen.Return();
            return (Func<IDataReaderDeserializer, ExecutionContext, object>)dynamicMethod.CreateDelegate(typeof(Func<IDataReaderDeserializer, ExecutionContext, object>));
        }
    }
}
