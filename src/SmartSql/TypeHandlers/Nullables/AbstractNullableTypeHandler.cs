using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;

namespace SmartSql.TypeHandlers
{
    public abstract class AbstractNullableTypeHandler<TProperty, TField> : AbstractTypeHandler<TProperty, TField>
    {
        public override TProperty GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            if (dataReader.IsDBNull(columnIndex)) { return Default; }
            return GetValueWhenNotNull(dataReader, columnIndex);
        }

        protected virtual TProperty GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return (TProperty)dataReader.GetValue(columnIndex);
        }
    }
}
