using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SmartSql.Common
{
    public static class ObjectExtension
    {
        public static Object GetValue(this Object obj, String propertyName)
        {
            if (obj is Dapper.DynamicParameters)
            {
                return (obj as Dapper.DynamicParameters).Get<Object>(propertyName);
            }
            else
            {
                return obj?.GetType().GetProperty(propertyName)?.GetValue(obj);
            }
        }
    }
}
