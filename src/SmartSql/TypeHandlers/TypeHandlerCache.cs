using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace SmartSql.TypeHandlers
{
    /// <summary>
    /// 静态TypeHandler缓存
    /// 此处使用静态类可能产生多SmartSql实例TypeHandler冲突的问题
    /// 后面可能优化为创建动态代理缓存类，Handler缓存到动态类字段中
    /// public static ITypeHandler<TProperty> {TMappedTypeName} ;
    /// </summary>
    /// <typeparam name="TProperty"></typeparam>
    /// <typeparam name="TField"></typeparam>
    public static class TypeHandlerCache<TProperty, TField>
    {
        public static ITypeHandler<TProperty, TField> Handler
        {
            get;
            private set;
        }
        public static void SetParameter(IDataParameter dataParameter, object parameterValue)
        {
            Handler.SetParameter(dataParameter, parameterValue);
        }
        public static TProperty GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return Handler.GetValue(dataReader, columnIndex, targetType);
        }
        internal static void SetHandler(ITypeHandler<TProperty, TField> handler)
        {
            Handler = handler;
        }
    }
}
