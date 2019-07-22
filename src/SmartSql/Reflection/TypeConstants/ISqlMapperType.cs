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
            public static readonly MethodInfo Execute = Type.GetMethod(nameof(ISqlMapper.Execute));
            public static readonly MethodInfo ExecuteScalar = Type.GetMethod(nameof(ISqlMapper.ExecuteScalar));
            public static readonly MethodInfo Query = Type.GetMethod(nameof(ISqlMapper.Query));
            public static readonly MethodInfo QuerySingle = Type.GetMethod(nameof(ISqlMapper.QuerySingle));
            public static readonly MethodInfo GetDataSet = Type.GetMethod(nameof(ISqlMapper.GetDataSet));
            public static readonly MethodInfo GetDataTable = Type.GetMethod(nameof(ISqlMapper.GetDataTable));

            public static readonly MethodInfo ExecuteAsync = Type.GetMethod(nameof(ISqlMapper.ExecuteAsync));
            public static readonly MethodInfo ExecuteScalarAsync = Type.GetMethod(nameof(ISqlMapper.ExecuteScalarAsync));
            public static readonly MethodInfo QueryAsync = Type.GetMethod(nameof(ISqlMapper.QueryAsync));
            public static readonly MethodInfo QuerySingleAsync = Type.GetMethod(nameof(ISqlMapper.QuerySingleAsync));
            public static readonly MethodInfo GetDataSetAsync = Type.GetMethod(nameof(ISqlMapper.GetDataSetAsync));
            public static readonly MethodInfo GetDataTableAsync = Type.GetMethod(nameof(ISqlMapper.GetDataTableAsync));
        }
    }
}
