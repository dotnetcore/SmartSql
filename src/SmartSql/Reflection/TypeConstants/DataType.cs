using SmartSql.Data;
using System;
using System.Collections;
using System.Data;

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
    }
}
