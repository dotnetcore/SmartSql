using SmartSql.Reflection.TypeConstants;
using System;
using System.Reflection;
using System.Reflection.Emit;
using SmartSql.Utils;

namespace SmartSql.Reflection.PropertyAccessor
{
    public class EmitGetAccessorFactory : IGetAccessorFactory
    {
        public Func<object, object> Create(Type targetType, string propertyName)
        {
            var propertyInfo = targetType.GetProperty(propertyName);
            return Create(propertyInfo);
        }

        public Func<object, object> Create(PropertyInfo propertyInfo)
        {
            return CacheUtil<EmitGetAccessorFactory, PropertyInfo, Func<object, object>>
                .GetOrAdd(propertyInfo, CreateImpl);
        }

        private Func<object, object> CreateImpl(PropertyInfo propertyInfo)
        {
            var dynamicMethod = new DynamicMethod("Get_" + Guid.NewGuid().ToString("N"), CommonType.Object,
                new[] {CommonType.Object}, propertyInfo.DeclaringType, true);
            var ilGen = dynamicMethod.GetILGenerator();
            ilGen.LoadArg(0);
            ilGen.Call(propertyInfo.GetGetMethod());
            if (propertyInfo.PropertyType.IsValueType)
            {
                ilGen.Box(propertyInfo.PropertyType);
            }

            ilGen.Return();
            return (Func<object, object>) dynamicMethod.CreateDelegate(typeof(Func<object, object>));
        }
    }
}