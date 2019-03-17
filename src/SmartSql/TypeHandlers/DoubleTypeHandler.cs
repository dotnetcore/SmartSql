using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class DoubleTypeHandler : AbstractTypeHandler<Double>
    {
        public override Double GetValue(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetDouble(columnIndex);
        }
    }
}
