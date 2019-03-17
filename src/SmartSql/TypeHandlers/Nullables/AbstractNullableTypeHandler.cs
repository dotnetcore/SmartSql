using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;

namespace SmartSql.TypeHandlers
{
    public abstract class AbstractNullableTypeHandler<T> : AbstractTypeHandler<T>
    {
        public override T GetValue(DataReaderWrapper dataReader, int columnIndex)
        {
            if (dataReader.IsDBNull(columnIndex)) { return Default; }
            return GetValueWhenNotNull(dataReader, columnIndex);
        }

        protected virtual T GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return (T)dataReader.GetValue(columnIndex);
        }
    }
}
