using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
namespace SmartSql.Common
{
    public static class ObjectExtension
    {
        public static Object GetValue(this Object obj, String propertyName)
        {
            var dyParams = (obj as Dapper.DynamicParameters);
            if (dyParams.ParameterNames.Contains(propertyName))
            {
                return dyParams.Get<Object>(propertyName);
            }
            return null;
        }
    }
}
