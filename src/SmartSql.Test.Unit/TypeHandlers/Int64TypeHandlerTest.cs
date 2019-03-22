using SmartSql.TypeHandlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Test.Unit.TypeHandlers
{
    public class Int64TypeHandlerTest
    {
        public void Test(Data.DataReaderWrapper dataReader)
        {

            var typeHandler = new Int64TypeHandler();

            var val = typeHandler.GetValue(dataReader, 1, typeof(long));

        }
    }
}
