using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SmartSql.Reflection.TypeConstants
{
    public static class ISqlMapperType
    {
        public static readonly Type Type = typeof(ISqlMapper);
        public static class Method
        {
            public static readonly MethodInfo Execute = Type.GetMethod("Execute");
            public static readonly MethodInfo ExecuteScalar = Type.GetMethod("ExecuteScalar");
            public static readonly MethodInfo Query = Type.GetMethod("Query");
            public static readonly MethodInfo QuerySingle = Type.GetMethod("QuerySingle");
            public static readonly MethodInfo GetDataSet = Type.GetMethod("GetDataSet");
            public static readonly MethodInfo GetDataTable = Type.GetMethod("GetDataTable");

            public static readonly MethodInfo ExecuteAsync = Type.GetMethod("ExecuteAsync");
            public static readonly MethodInfo ExecuteScalarAsync = Type.GetMethod("ExecuteScalarAsync");
            public static readonly MethodInfo QueryAsync = Type.GetMethod("QueryAsync");
            public static readonly MethodInfo QuerySingleAsync = Type.GetMethod("QuerySingleAsync");
            public static readonly MethodInfo GetDataSetAsync = Type.GetMethod("GetDataSetAsync");
            public static readonly MethodInfo GetDataTableAsync = Type.GetMethod("GetDataTableAsync");
        }
    }
}
