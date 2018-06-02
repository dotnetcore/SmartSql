using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace SmartSql.Utils
{
    public static class ObjectUtils
    {
        private static readonly object _syncObj = new object();
        private static Dictionary<string, Func<object, Dictionary<string, object>>> _cachedConvert = new Dictionary<string, Func<object, Dictionary<string, object>>>();

        public static Dictionary<string, object> ToDictionary(object sourceObj, bool ignorePropNameCase)
        {
            var dicConvert = GetDictionaryConvert(sourceObj.GetType(), ignorePropNameCase);
            return dicConvert(sourceObj);
        }

        public static Func<object, Dictionary<string, object>> GetDictionaryConvert(Type sourceType, bool ignorePropNameCase)
        {
            string key = $"{sourceType.GUID.ToString("N")}_{ignorePropNameCase}";
            if (!_cachedConvert.ContainsKey(key))
            {
                lock (_syncObj)
                {
                    if (!_cachedConvert.ContainsKey(key))
                    {
                        var impl = CreateDictionaryConvertConvertImpl(sourceType, ignorePropNameCase);
                        _cachedConvert.Add(key, impl);
                    }
                }
            }
            return _cachedConvert[key];
        }

        private static readonly Type _dicType = typeof(Dictionary<string, object>);
        private static readonly ConstructorInfo _dicCtor = _dicType.GetConstructor(new Type[] { typeof(int), typeof(IEqualityComparer<string>) });
        private static readonly MethodInfo _addItemDicMentod = _dicType.GetMethod("Add");
        private static readonly MethodInfo _get_CurrentCultureIgnoreCase_StringComparer = typeof(StringComparer).GetMethod("get_CurrentCultureIgnoreCase");
        private static readonly MethodInfo _get_CurrentCulture_StringComparer = typeof(StringComparer).GetMethod("get_CurrentCulture");
        private static Func<object, Dictionary<string, object>> CreateDictionaryConvertConvertImpl(Type sourceType, bool ignorePropNameCase)
        {
            Type returnType = _dicType;
            var addItemMethod = _addItemDicMentod;
            var sourceProps = sourceType.GetProperties().Where(p => p.CanRead);
            var dynamicMethod = new DynamicMethod("ObjToDic" + Guid.NewGuid().ToString("N"), returnType, new[] { typeof(object) }, sourceType, true);
            var iLGenerator = dynamicMethod.GetILGenerator();
            iLGenerator.DeclareLocal(returnType);
            EmitUtils.LoadInt32(iLGenerator, sourceProps.Count());
            iLGenerator.Emit(OpCodes.Call, ignorePropNameCase ? _get_CurrentCultureIgnoreCase_StringComparer : _get_CurrentCulture_StringComparer);
            iLGenerator.Emit(OpCodes.Newobj, _dicCtor);
            iLGenerator.Emit(OpCodes.Stloc_0);
            foreach (var prop in sourceProps)
            {
                iLGenerator.Emit(OpCodes.Ldloc_0); //[dic]
                iLGenerator.Emit(OpCodes.Ldstr, prop.Name);//[dic][prop-name]
                iLGenerator.Emit(OpCodes.Ldarg_0);//[dic][prop-name][sourceObj]
                iLGenerator.Emit(OpCodes.Call, prop.GetMethod);//[dic][prop-name][prop-value]
                if (prop.PropertyType.IsValueType)
                {
                    iLGenerator.Emit(OpCodes.Box, prop.PropertyType);
                }
                iLGenerator.Emit(OpCodes.Call, addItemMethod);//[empty]
            }
            iLGenerator.Emit(OpCodes.Ldloc_0);
            iLGenerator.Emit(OpCodes.Ret);
            var funcType = System.Linq.Expressions.Expression.GetFuncType(typeof(object), returnType);
            return (Func<object, Dictionary<string, object>>)dynamicMethod.CreateDelegate(funcType);
        }

    }
}
