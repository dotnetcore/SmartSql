using SmartSql.TypeHandlers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace SmartSql.Data
{
    public class SqlParameter
    {
        private DbParameter _sourceParameter;
        public SqlParameter(string name, object val, Type parameterType)
        {
            Name = name;
            Value = val;
            ParameterType = parameterType;
        }
        public string Name { get; set; }
        public object Value { get; set; }
        public Type ParameterType { get; set; }
        public Action<SqlParameter> OnSetSourceParameter { get; set; }

        public DbParameter SourceParameter
        {
            get => _sourceParameter; set
            {
                _sourceParameter = value;
                OnSetSourceParameter?.Invoke(this);
            }
        }
        public byte? Precision { get; set; }
        public byte? Scale { get; set; }
        public int? Size { get; set; }
        public DbType? DbType { get; set; }
        public ParameterDirection? Direction { get; set; }
        public ITypeHandler TypeHandler { get; set; }
    }
}
