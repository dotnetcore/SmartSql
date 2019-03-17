using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;

namespace SmartSql.TypeHandlers
{
    public abstract class AbstractTypeHandler<T> : ITypeHandler<T>
    {
        public virtual string Name { get; }
        public Type MappedType { get; }
        public T Default { get; }
        public bool IsNullable { get; }

        protected AbstractTypeHandler()
        {
            MappedType = typeof(T);
            Default = default(T);
            IsNullable = Default == null;
            Name = MappedType.Name;
        }

        public abstract T GetValue(DataReaderWrapper dataReader, int columnIndex);

        public virtual T GetValue(DataReaderWrapper dataReader, string columnName)
        {
            var columnIndex = dataReader.GetOrdinal(columnName);
            return GetValue(dataReader, columnIndex);
        }

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
