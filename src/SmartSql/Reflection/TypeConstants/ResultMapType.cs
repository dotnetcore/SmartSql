using SmartSql.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SmartSql.Reflection.TypeConstants
{
    public static class ResultMapType
    {
        public static readonly Type Type = typeof(ResultMap);

        public class Method
        {
            public static readonly MethodInfo GetHandler = Type.GetMethod("GetHandler");
        }
    }
}
