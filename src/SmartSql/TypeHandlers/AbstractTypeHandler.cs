using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;

namespace SmartSql.TypeHandlers
{
    public abstract class AbstractTypeHandler<TProperty, TField> : ITypeHandler<TProperty, TField>
    {
        public Type PropertyType { get; }
        public Type FieldType { get; }
        public TProperty Default { get; }
        public bool IsNullable { get; }

        protected AbstractTypeHandler()
        {
            PropertyType = typeof(TProperty);
            FieldType = typeof(TField);
            Default = default(TProperty);
            IsNullable = Default == null;
        }

        public abstract TProperty GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType);

        public virtual void Initialize(IDictionary<string, object> parameters)
        {
            if (parameters == null) { return; }
        }

        public virtual void SetParameter(IDataParameter dataParameter, object parameterValue)
        {
            if (parameterValue != null)
            {
                SetParameterWhenNotNull(dataParameter, parameterValue);
            }
            else
            {
                if (IsNullable)
                {
                    dataParameter.Value = DBNull.Value;
                }
                else
                {
                    dataParameter.Value = Default;
                }
            }
        }
        protected virtual void SetParameterWhenNotNull(IDataParameter dataParameter, object parameterValue)
        {
            dataParameter.Value = parameterValue;
        }

    }
}
