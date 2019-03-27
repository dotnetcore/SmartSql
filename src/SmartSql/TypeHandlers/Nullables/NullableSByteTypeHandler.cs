using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class NullableSByteTypeHandler : AbstractNullableTypeHandler<SByte?, SByte>
    {
        protected override SByte? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetFieldValue<SByte>(columnIndex);
        }
    }
    public class NullableSByteAnyTypeHandler : AbstractNullableTypeHandler<SByte?, AnyFieldType>
    {
        protected override SByte? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return Convert.ToSByte(dataReader.GetValue(columnIndex));
        }
    }
}
