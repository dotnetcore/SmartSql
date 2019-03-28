using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public static class PropertyTypeHandlerCache<TProperty>
    {
        public static ITypeHandler Handler
        {
            get;
            private set;
        }
        public static void SetParameter(IDataParameter dataParameter, object parameterValue)
        {
            Handler.SetParameter(dataParameter, parameterValue);
        }
        internal static void SetHandler(ITypeHandler handler)
        {
            Handler = handler;
        }
    }
}
