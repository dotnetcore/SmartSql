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
            public static readonly MethodInfo ToSingle = Type.GetMethod(nameof(IDataReaderDeserializer.ToSingle));
            public static readonly MethodInfo ToList = Type.GetMethod(nameof(IDataReaderDeserializer.ToList));
            public static MethodInfo MakeGenericToSingle(Type resultType)
            {
                return ToSingle.MakeGenericMethod(resultType);
            }
            public static MethodInfo MakeGenericToList(Type resultType)
            {
                return ToList.MakeGenericMethod(resultType);
            }
        }
    }
}
