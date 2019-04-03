using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using SmartSql.Deserializer;
using SmartSql.Exceptions;
using SmartSql.Reflection.TypeConstants;

namespace SmartSql.Reflection
{
    public class ValueTupleConvert
    {
        private static readonly object lockObj = new object();
        private static readonly IDictionary<Type, Func<object[], object>> _cachedImpl = new Dictionary<Type, Func<object[], object>>();

        public static object Convert(Type valueTupleType, object[] argVals)
        {
            return GetImpl(valueTupleType)(argVals);
        }
        private static Func<object[], object> GetImpl(Type valueTupleType)
        {
            if (!_cachedImpl.ContainsKey(valueTupleType))
            {
                lock (lockObj)
                {
                    if (!_cachedImpl.ContainsKey(valueTupleType))
                    {
                        var impl = CreateImpl(valueTupleType);
                        _cachedImpl.Add(valueTupleType, impl);
                    }
                }
            }
            return _cachedImpl[valueTupleType];
        }
        private static Func<object[], object> CreateImpl(Type valueTupleType)
        {
            var genericTypeArguments = valueTupleType.GenericTypeArguments;
            var createVT = CommonType.GetValueTupleCreateMethod(genericTypeArguments);
            var dynamicMethod = new DynamicMethod("ValueTupleConvert_" + Guid.NewGuid().ToString("N"), CommonType.Object, new[] { CommonType.ObjectArray });

            var ilGen = dynamicMethod.GetILGenerator();
            for (int i = 0; i < genericTypeArguments.Length; i++)
            {
                Type argType = genericTypeArguments[i];
                ilGen.LoadArg(0);
                ilGen.LoadInt32(i);
                ilGen.LoadElement(CommonType.Object);
                if (argType.IsValueType)
                {
                    ilGen.Unbox(argType);
                    ilGen.LoadValueIndirect(argType);
                }
            }
            ilGen.Emit(OpCodes.Call, createVT);
            ilGen.Box(valueTupleType);
            ilGen.Return();
            return (Func<object[], object>)dynamicMethod.CreateDelegate(typeof(Func<object[], object>));
        }
    }
}
