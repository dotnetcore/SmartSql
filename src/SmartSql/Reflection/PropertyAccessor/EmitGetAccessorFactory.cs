using SmartSql.Reflection.TypeConstants;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using SmartSql.Configuration.Tags;
using SmartSql.Exceptions;
using SmartSql.Utils;

namespace SmartSql.Reflection.PropertyAccessor
{
    public class EmitGetAccessorFactory : IGetAccessorFactory
    {
        public static readonly IGetAccessorFactory Instance = new EmitGetAccessorFactory();

        private EmitGetAccessorFactory()
        {
        }

        public bool TryCreate(Type targetType, PropertyTokenizer propertyTokenizer,
            out Func<object, object> getMethodImpl)
        {
            var key = $"{targetType.FullName}__{propertyTokenizer.FullName}";
            getMethodImpl = CacheUtil<EmitGetAccessorFactory, String, Func<object, object>>
                .GetOrAdd(key, s =>
                {
                    TryCreateImpl(targetType, propertyTokenizer, out var getMethod);
                    return getMethod;
                });
            return getMethodImpl != null;
        }

        private bool TryCreateImpl(Type targetType, PropertyTokenizer propertyTokenizer,
            out Func<object, object> getMethodImpl)
        {
            getMethodImpl = null;
            var dynamicMethod = new DynamicMethod("Get_" + Guid.NewGuid().ToString("N"), CommonType.Object,
                new[] {CommonType.Object}, targetType, true);
            var ilGen = dynamicMethod.GetILGenerator();
            ilGen.LoadArg(0);
            var propertyType = targetType;
            do
            {
                 var current = propertyTokenizer.Current;
                switch (current.Mode)
                {
                    case AccessMode.Get:
                    {
                        var propertyInfo = propertyType.GetProperty(current.Name);

                        if (propertyInfo == null)
                        {
                            return false;
                        }

                        ilGen.Call(propertyInfo.GetMethod);
                        propertyType = propertyInfo.PropertyType;
                        if (propertyType.IsValueType)
                        {
                            ilGen.Box(propertyType);
                        }

                        break;
                    }

                    case AccessMode.IndexerGet:
                    {
                        var propertyInfo = propertyType.GetProperty(current.Name);
                        if (propertyInfo == null)
                        {
                            return false;
                        }

                        ilGen.Call(propertyInfo.GetMethod);

                        MethodInfo indexerGet;
                        if (propertyInfo.PropertyType.IsArray)
                        {
                            indexerGet = propertyInfo.PropertyType.GetMethod("Get", new[] {CommonType.Int32});
                        }
                        else
                        {
                            indexerGet = propertyInfo.PropertyType.GetMethod("get_Item");
                        }

                        if (indexerGet == null)
                        {
                            throw new SmartSqlException($"can not find Get Indexer:{propertyInfo.PropertyType}");
                        }


                        var indexerGetParamType = indexerGet.GetParameters().First().ParameterType;

                        if (indexerGetParamType == CommonType.String)
                        {
                            ilGen.LoadString(current.Index);
                        }
                        else if (indexerGetParamType == CommonType.Int32)
                        {
                            if (!Int32.TryParse(current.Index, out var idx))
                            {
                                throw new SmartSqlException($"Index:[{current.Index}] can not convert to Int32.");
                            }

                            ilGen.LoadInt32(idx);
                        }

                        ilGen.Call(indexerGet);
                        propertyType = indexerGet.ReturnType;
                        if (propertyType.IsValueType)
                        {
                            ilGen.Box(propertyType);
                        }

                        break;
                    }
                }
            } while (propertyTokenizer.MoveNext());

            ilGen.Return();
            getMethodImpl = (Func<object, object>) dynamicMethod.CreateDelegate(typeof(Func<object, object>));
            return true;
        }

        public bool TryCreate(Type targetType, string fullName, out Func<object, object> getMethodImpl)
        {
            return TryCreate(targetType, new PropertyTokenizer(fullName), out getMethodImpl);
        }

        public Func<object, object> Create(PropertyInfo propertyInfo)
        {
            return CacheUtil<EmitGetAccessorFactory, PropertyInfo, Func<object, object>>
                .GetOrAdd(propertyInfo, CreateImpl);
        }

        private Func<object, object> CreateImpl(PropertyInfo propertyInfo)
        {
            if (propertyInfo.GetMethod == null)
            {
                throw new SmartSqlException(
                    $"Can not find GetMethod -> Type:[{propertyInfo.DeclaringType.FullName}],PropertyInfo :[{propertyInfo.Name}]. ");
            }

            var dynamicMethod = new DynamicMethod("Get_" + Guid.NewGuid().ToString("N"), CommonType.Object,
                new[] {CommonType.Object}, propertyInfo.DeclaringType, true);
            var ilGen = dynamicMethod.GetILGenerator();
            ilGen.LoadArg(0);
            ilGen.Call(propertyInfo.GetMethod);
            if (propertyInfo.PropertyType.IsValueType)
            {
                ilGen.Box(propertyInfo.PropertyType);
            }

            ilGen.Return();
            return (Func<object, object>) dynamicMethod.CreateDelegate(typeof(Func<object, object>));
        }
    }
}