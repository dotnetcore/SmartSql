using SmartSql.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration.Tags
{
    public class IsEqual : StringCompareTag
    {
        public override bool IsCondition(RequestContext context)
        {
            var reqVal = GetPropertyValue(context);
            if (reqVal == null) { return false; }
            string reqValStr = string.Empty;
            if (reqVal is Enum)
            {
                reqValStr = Convert.ToInt64(reqVal).ToString();
            }
            else
            {
                reqValStr = reqVal.ToString();
            }
            return reqValStr.Equals(CompareValue);
        }
    }
}
