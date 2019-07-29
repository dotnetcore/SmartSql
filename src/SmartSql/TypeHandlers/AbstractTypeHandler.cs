using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;

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
            if (parameters == null)
            {
                return;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual object GetSetParameterValue(object parameterValue)
        {
            if (parameterValue != null)
            {
                return GetSetParameterValueWhenNotNull(parameterValue);
            }

            if (IsNullable)
            {
                return DBNull.Value;
            }

            return Default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual object GetSetParameterValueWhenNotNull(object parameterValue)
        {
            return parameterValue;
        }

        public virtual void SetParameter(IDataParameter dataParameter, object parameterValue)
        {
            dataParameter.Value = GetSetParameterValue(parameterValue);
        }
    }
}