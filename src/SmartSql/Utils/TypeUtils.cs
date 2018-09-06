using SmartSql.Abstractions;
using SmartSql.Abstractions.TypeHandler;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace SmartSql.Utils
{
    public static class TypeUtils
    {
        public static readonly Type DataReaderType = typeof(IDataReader);
        public static readonly Type DataRecordType = typeof(IDataRecord);
        public static readonly MethodInfo IsDBNullMethod = DataRecordType.GetMethod(nameof(IDataRecord.IsDBNull));
        public static readonly MethodInfo GetValueMethod_DataRecord = DataRecordType.GetMethod(nameof(IDataReader.GetValue));

        public static readonly Type ObjectType = typeof(object);
        public static readonly Type TypeType = typeof(Type);
        public static readonly Type IntType = typeof(int);
        public static readonly Type BooleanType = typeof(bool);
        public static readonly Type StringType = typeof(string);
        public static readonly Type EnumType = typeof(Enum);

        public static readonly MethodInfo GetType_Object = ObjectType.GetMethod(nameof(Object.GetType));
        public static readonly MethodInfo GetTypeFromHandleMethod = TypeType.GetMethod(nameof(Type.GetTypeFromHandle));

        public static readonly Type TypeHandlerType = typeof(ITypeHandler);
        public static readonly MethodInfo GetValueMethod_TypeHandler = TypeHandlerType.GetMethod(nameof(ITypeHandler.GetValue), new Type[] { DataReaderType, IntType, TypeType });


        public static readonly Type RequestContextType = typeof(RequestContext);
    }
}
