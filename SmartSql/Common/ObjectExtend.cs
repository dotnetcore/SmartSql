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
            return obj?.GetType().GetProperty(propertyName)?.GetValue(obj);
        }
    }
}
