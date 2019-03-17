using SmartSql.Data;
using System;
using System.Collections;
using System.Data;
using System.Reflection;

namespace SmartSql.Reflection.TypeConstants
{
    public static class DataType
    {
        public readonly static Type Enumerable = typeof(IEnumerable);
        public readonly static Type DataSet = typeof(DataSet);
        public readonly static Type DataTable = typeof(DataTable);
        public readonly static Type Dictionary = typeof(IDictionary);
        public readonly static Type DataReaderWrapper = typeof(DataReaderWrapper);
    }
}
