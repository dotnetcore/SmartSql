using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class NullableTimeSpanTypeHandler : AbstractNullableTypeHandler<TimeSpan?>
    {
        //protected override void SetParameterWhenNotNull(IDataParameter dataParameter, object parameterValue)
        //{
        //    dataParameter.Value = ((TimeSpan)parameterValue).Ticks;
        //}
        //protected override TimeSpan? GetValueWhenNotNull(IDataReader dataReader, int columnIndex)
        //{
        //    return new TimeSpan(dataReader.GetInt64(columnIndex));
        //    //return new TimeSpan(Convert.ToInt64(dataReader.GetInt64(columnIndex)));
        //}
    }
}
