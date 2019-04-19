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
