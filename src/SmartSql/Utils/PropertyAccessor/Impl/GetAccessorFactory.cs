using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
namespace SmartSql.Utils.PropertyAccessor.Impl
{
    public class GetAccessorFactory : IGetAccessorFactory
    {
        public static IGetAccessorFactory Instance = new GetAccessorFactory();

        private readonly Dictionary<string, Func<object, object>> _cachedGetImpl = new Dictionary<string, Func<object, object>>();
        public Func<object, object> CreateGet(Type targetType, string propertyName, bool ignorePropertyCase)
        {
            var implName = GetGetImplName(targetType, propertyName, ignorePropertyCase);
            if (!_cachedGetImpl.ContainsKey(implName))
            {
                lock (this)
                {
                    if (!_cachedGetImpl.ContainsKey(implName))
                    {
                        var getImpl = CreateGetImpl(targetType, propertyName, ignorePropertyCase);
                        _cachedGetImpl.Add(implName, getImpl);
                    }
                }
            }
            return _cachedGetImpl[implName];
        }
        private Func<object, object> CreateGetImpl(Type targetType, string propertyName, bool ignorePropertyCase)
        {
            var implName = GetGetImplName(targetType, propertyName, ignorePropertyCase);
            var dynamicMethod = new DynamicMethod(implName, TypeUtils.ObjectType, new[] { TypeUtils.ObjectType }, TypeUtils.ObjectType, true);
            var iLGenerator = dynamicMethod.GetILGenerator();

            var property = GetProperty(targetType, propertyName, ignorePropertyCase);

            if (property == null)
            {
                return (target) => { return null; };
            }
            var getProperty = property.GetMethod;
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Call, getProperty);
            if (getProperty.ReturnType.IsValueType)
            {
                iLGenerator.Emit(OpCodes.Box, getProperty.ReturnType);
            }
            iLGenerator.Emit(OpCodes.Ret);
            var funcType = Expression.GetFuncType(TypeUtils.ObjectType, TypeUtils.ObjectType);
            return (Func<object, object>)dynamicMethod.CreateDelegate(funcType);
        }

        private string GetGetImplName(Type targetType, string propertyName, bool ignorePropertyCase)
        {
            return $"SmartSql_Get_{targetType.FullName}_{propertyName}_{ignorePropertyCase}";
        }
        
        private PropertyInfo GetProperty(Type targetType, string propertyName, bool ignorePropertyCase)
        {
            return targetType.GetProperties()
                 .FirstOrDefault(p => String.Compare(p.Name, propertyName, ignorePropertyCase) == 0);
        }
    }
}
