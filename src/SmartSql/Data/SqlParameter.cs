using SmartSql.TypeHandlers;
using System;
using System.Data;
using System.Data.Common;

namespace SmartSql.Data
{
    public class SqlParameter
    {
        private DbParameter _sourceParameter;
        private object _value;

        public SqlParameter()
        {
        }

        public SqlParameter(string name, object val)
        {
            Name = name;
            Value = val;
            if (val != null)
            {
                ParameterType = val.GetType();
            }
        }

        public SqlParameter(string name, object val, Type parameterType)
        {
            Name = name;
            Value = val;
            ParameterType = parameterType;
        }

        public string Name { get; set; }

        public object Value
        {
            get => _value;
            set
            {
                _value = value;
                if (value != null)
                {
                    ParameterType = value.GetType();
                }
            }
        }

        public Type ParameterType { get; set; }
        internal Action<SqlParameter> OnSetSourceParameter { get; set; }

        public DbParameter SourceParameter
        {
            get => _sourceParameter;
            set
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