using SmartSql.Data;
using System;
using System.Collections;
using System.Data;
using System.Reflection;

namespace SmartSql.Reflection.TypeConstants
{
    public static class DataType
    {
        public static readonly Type Enumerable = typeof(IEnumerable);
        public static readonly Type DataSet = typeof(DataSet);
        public static readonly Type DataTable = typeof(DataTable);
        public static readonly Type Dictionary = typeof(IDictionary);
        public static readonly Type DataReaderWrapper = typeof(DataReaderWrapper);
        public static readonly Type DynamicRow = typeof(DynamicRow);

        public class Method
        {
            public static readonly MethodInfo IsDBNull = DataReaderWrapper.GetMethod(nameof(IsDBNull));
        }
    }
}