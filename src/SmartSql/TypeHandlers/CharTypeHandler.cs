using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class CharTypeHandler : AbstractTypeHandler<Char>
    {
        public override Char GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return dataReader.GetString(columnIndex)[0];
        }
    }
}
