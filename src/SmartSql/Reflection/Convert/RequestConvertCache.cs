using SmartSql.Data;
using SmartSql.Reflection.TypeConstants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using SmartSql.Annotations;
using SmartSql.Exceptions;
using SmartSql.TypeHandlers;

namespace SmartSql.Reflection.Convert
{
    /// <summary>
    /// TODO add SmartSqlAlias parameter
    /// </summary>
    public static class RequestConvertCache<TRequest>
    {
        public static Func<object, SqlParameterCollection> Convert => RequestConvertCache<TRequest, object>.Convert;
    }

    public static class RequestConvertCache<TRequest, TIgnoreCase>
    {
        private static Lazy<Func<object, SqlParameterCollection>> _lazyConvert =
            new Lazy<Func<object, SqlParameterCollection>>(BuildConvert);

        public static Func<object, SqlParameterCollection> Convert => _lazyConvert.Value;

        private static Func<object, SqlParameterCollection> BuildConvert()
        {
            var requestType = typeof(TRequest);
            var sourceProps = requestType.GetProperties().Where(p => p.CanRead);
            var dynamicMethod = new DynamicMethod(Guid.NewGuid().ToString("N"), SqlParameterType.SqlParameterCollection,
                new[] {CommonType.Object}, requestType, true);
            var ilGen = dynamicMethod.GetILGenerator();
            ilGen.DeclareLocal(SqlParameterType.SqlParameterCollection);
            var ignoreCase = typeof(TIgnoreCase) == typeof(IgnoreCaseType);
            ilGen.LoadInt32(ignoreCase ? 1 : 0);
            ilGen.New(SqlParameterType.Ctor.SqlParameterCollection);
            ilGen.StoreLocalVar(0);
            foreach (var prop in sourceProps)
            {
                ilGen.LoadLocalVar(0);
                ilGen.LoadString(prop.Name);
                ilGen.LoadArg(0);
                ilGen.Call(prop.GetMethod);
                if (prop.PropertyType.IsValueType)
                {
                    ilGen.Box(prop.PropertyType);
                }

                ilGen.LoadType(prop.PropertyType);
                ilGen.New(SqlParameterType.Ctor.SqlParameter);

                #region  Ensure TypeHanlder

                ilGen.Dup();
                var column = prop.GetCustomAttribute<ColumnAttribute>();
                if (column != null && !String.IsNullOrEmpty(column.TypeHandler))
                {
                    var typeHandlerField =
                        NamedTypeHandlerCache.GetTypeHandlerField(column.Alias, column.TypeHandler);
                    if (typeHandlerField == null)
                    {
                        throw new SmartSqlException(
                            $"Can not find NamedTypeHandler SmartSql.Alias:[{column.Alias}],Name :[{column.TypeHandler}].");
                    }

                    ilGen.FieldGet(typeHandlerField);
                }
                else
                {
                    MethodInfo getHandlerMethod = null;
                    if (column?.FieldType != null)
                    {
                        getHandlerMethod = TypeHandlerCacheType.GetHandlerMethod(prop.PropertyType, column.FieldType);
                    }

                    if (getHandlerMethod == null)
                    {
                        getHandlerMethod = PropertyTypeHandlerCacheType.GetHandlerMethod(prop.PropertyType);
                    }

                    ilGen.Call(getHandlerMethod);
                }

                ilGen.Call(SqlParameterType.Method.SetTypeHandler);
                ilGen.Call(SqlParameterType.Method.Add);

                #endregion
            }

            ilGen.LoadLocalVar(0);
            ilGen.Return();
            return (Func<object, SqlParameterCollection>) dynamicMethod.CreateDelegate(
                typeof(Func<object, SqlParameterCollection>));
        }
    }
}