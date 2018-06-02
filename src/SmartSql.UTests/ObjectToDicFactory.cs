using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace SmartSql.UTests
{
    public class ObjectToDicFactory
    {
        private IDictionary<Type, Func<object, IDictionary<string, object>>> _cachedConvert = new Dictionary<Type, Func<object, IDictionary<string, object>>>();

        public Func<object, IDictionary<string, object>> GetConvert(Type sourceType)
        {
            if (!_cachedConvert.ContainsKey(sourceType))
            {
                lock (this)
                {
                    if (!_cachedConvert.ContainsKey(sourceType))
                    {
                        var impl = CreateConvertImpl(sourceType);
                        _cachedConvert.Add(sourceType, impl);
                    }
                }
            }
            return _cachedConvert[sourceType];
        }
        private Func<object, IDictionary<string, object>> CreateConvertImpl(Type sourceType)
        {
            Type returnType = typeof(IDictionary<string, object>);
            var addItemMethod = returnType.GetMethod("Add");
            var sourceProps = sourceType.GetProperties().Where(p => p.CanRead);

            var dynamicMethod = new DynamicMethod("ObjToDic" + Guid.NewGuid().ToString("N"), returnType, new[] { typeof(object) }, sourceType, true);
            var iLGenerator = dynamicMethod.GetILGenerator();
            iLGenerator.DeclareLocal(returnType);
            var dicCtor = typeof(Dictionary<string, object>).GetConstructor(new Type[] { typeof(int) });
            EmitInt32(iLGenerator, sourceProps.Count());
            iLGenerator.Emit(OpCodes.Newobj, dicCtor);
            iLGenerator.Emit(OpCodes.Stloc_0);

            foreach (var prop in sourceProps)
            {
                iLGenerator.Emit(OpCodes.Ldloc_0); //[dic]
                iLGenerator.Emit(OpCodes.Ldstr, prop.Name);//[dic][prop-name]
                iLGenerator.Emit(OpCodes.Ldarg_0);//[dic][prop-name][sourceObj]
                iLGenerator.Emit(OpCodes.Callvirt, prop.GetMethod);//[dic][prop-name][prop-value]
                if (prop.PropertyType.IsValueType)
                {
                    iLGenerator.Emit(OpCodes.Box, prop.PropertyType);
                }
                iLGenerator.Emit(OpCodes.Callvirt, addItemMethod);//[empty]
            }
            iLGenerator.Emit(OpCodes.Ldloc_0);
            iLGenerator.Emit(OpCodes.Ret);
            var funcType = System.Linq.Expressions.Expression.GetFuncType(typeof(object), returnType);
            return (Func<object, IDictionary<string, object>>)dynamicMethod.CreateDelegate(funcType);
        }

        private void EmitInt32(ILGenerator iLGenerator, int value)
        {
            switch (value)
            {
                case -1: iLGenerator.Emit(OpCodes.Ldc_I4_M1); break;
                case 0: iLGenerator.Emit(OpCodes.Ldc_I4_0); break;
                case 1: iLGenerator.Emit(OpCodes.Ldc_I4_1); break;
                case 2: iLGenerator.Emit(OpCodes.Ldc_I4_2); break;
                case 3: iLGenerator.Emit(OpCodes.Ldc_I4_3); break;
                case 4: iLGenerator.Emit(OpCodes.Ldc_I4_4); break;
                case 5: iLGenerator.Emit(OpCodes.Ldc_I4_5); break;
                case 6: iLGenerator.Emit(OpCodes.Ldc_I4_6); break;
                case 7: iLGenerator.Emit(OpCodes.Ldc_I4_7); break;
                case 8: iLGenerator.Emit(OpCodes.Ldc_I4_8); break;
                default:
                    if (value >= -128 && value <= 127)
                    {
                        iLGenerator.Emit(OpCodes.Ldc_I4_S, (sbyte)value);
                    }
                    else
                    {
                        iLGenerator.Emit(OpCodes.Ldc_I4, value);
                    }
                    break;
            }
        }
    }
}
