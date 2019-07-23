using SmartSql.Reflection.TypeConstants;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using SmartSql.Utils;

namespace SmartSql.Reflection.PropertyAccessor
{
    public class EmitSetAccessorFactory : ISetAccessorFactory
    {
        public static readonly ISetAccessorFactory Instance = new EmitSetAccessorFactory();

        private EmitSetAccessorFactory()
        {
        }
        public Action<object, object> Create(Type targetType, string propertyName)
        {
            var propertyInfo = targetType.GetProperty(propertyName);
            return Create(propertyInfo);
        }

        public Action<object, object> Create(PropertyInfo propertyInfo)
        {
            return CacheUtil<EmitSetAccessorFactory, PropertyInfo, Action<object, object>>
                   .GetOrAdd(propertyInfo, CreateImpl);
        }

        private Action<object, object> CreateImpl(PropertyInfo propertyInfo)
        {
            var dynamicMethod = new DynamicMethod("Set_" + Guid.NewGuid().ToString("N"), null, new[] { CommonType.Object, CommonType.Object }, propertyInfo.DeclaringType, true);
            var ilGen = dynamicMethod.GetILGenerator();
            ilGen.LoadArg(0);
            ilGen.LoadArg(1);
            if (propertyInfo.PropertyType.IsValueType)
            {
                ilGen.Unbox(propertyInfo.PropertyType);
                ilGen.LoadValueIndirect(propertyInfo.PropertyType);
            }
            ilGen.Call(propertyInfo.SetMethod);
            ilGen.Return();
            return (Action<object, object>)dynamicMethod.CreateDelegate(typeof(Action<object, object>));
        }
    }
}
