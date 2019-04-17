using SmartSql.Deserializer;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SmartSql.Reflection.TypeConstants
{
    public static class IDataReaderDeserializerType
    {
        public static readonly Type Type = typeof(IDataReaderDeserializer);
        public static class Method
        {
            public static readonly MethodInfo ToSinge = Type.GetMethod("ToSinge");
            public static readonly MethodInfo ToList = Type.GetMethod("ToList");
            public static MethodInfo MakeGenericToSinge(Type resultType)
            {
                return ToSinge.MakeGenericMethod(resultType);
            }
            public static MethodInfo MakeGenericToList(Type resultType)
            {
                return ToList.MakeGenericMethod(resultType);
            }
        }
    }
}
