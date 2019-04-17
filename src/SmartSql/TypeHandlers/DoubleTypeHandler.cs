using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class DoubleTypeHandler : AbstractTypeHandler<Double, Double>
    {
        public override Double GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return dataReader.GetDouble(columnIndex);
        }
    }
    public class DoubleAnyTypeHandler : AbstractTypeHandler<Double, AnyFieldType>
    {
        public override Double GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return Convert.ToDouble(dataReader.GetValue(columnIndex));
        }
    }
}
