using SmartSql.Data;
using SmartSql.Reflection.TypeConstants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace SmartSql.Reflection.Convert
{
    public class IgnoreCase { }
    public static class RequestConvertCache<TRequest>
    {
        public static Func<object, SqlParameterCollection> Convert { get; private set; }
        static RequestConvertCache()
        {
            Convert = RequestConvertCache<TRequest, object>.Convert;
        }
    }

    public static class RequestConvertCache<TRequest, TIgnoreCase>
    {
        public static Func<object, SqlParameterCollection> Convert { get; private set; }

        static RequestConvertCache()
        {
            BuildConvert();
        }

        private static void BuildConvert()
        {
            var requestType = typeof(TRequest);
            var sourceProps = requestType.GetProperties().Where(p => p.CanRead);
            var dynamicMethod = new DynamicMethod(Guid.NewGuid().ToString("N"), SqlParameterType.SqlParameterCollection, new[] { CommonType.Object }, requestType, true);
            var ilGen = dynamicMethod.GetILGenerator();
            ilGen.DeclareLocal(SqlParameterType.SqlParameterCollection);
            var ignoreCase = typeof(TIgnoreCase) == typeof(IgnoreCase);
            ilGen.LoadInt32(ignoreCase ? 1 : 0);
            ilGen.New(SqlParameterType.Ctor.SqlParameterCollection);
            ilGen.StoreLocalVar(0);
            foreach (var prop in sourceProps)
            {
                ilGen.LoadLocalVar(0);
                ilGen.LoadString(prop.Name);
                ilGen.LoadArg(0);
                ilGen.Emit(OpCodes.Call, prop.GetMethod);
                if (prop.PropertyType.IsValueType)
                {
                    ilGen.Box(prop.PropertyType);
                }
                ilGen.LoadType(prop.PropertyType);
                ilGen.New(SqlParameterType.Ctor.SqlParameter);
                ilGen.Dup();
                var getHandlerMethod = TypeHandlerCacheType.GetHandlerMethod(prop.PropertyType);
                ilGen.Call(getHandlerMethod);
                ilGen.Call(SqlParameterType.Method.SetTypeHandler);
                ilGen.Call(SqlParameterType.Method.Add);
            }
            ilGen.LoadLocalVar(0);
            ilGen.Return();
            Convert = (Func<object, SqlParameterCollection>)dynamicMethod.CreateDelegate(typeof(Func<object, SqlParameterCollection>));
        }
    }
}
