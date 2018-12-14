using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Utils
{
    public class EnumUtils
    {
        public static object ToRealValue(object enumVal)
        {
            return Convert.ChangeType(enumVal, Enum.GetUnderlyingType(enumVal.GetType()));
        }
    }
}
