using SmartSql.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration.Tags
{
    public class IsNotEqual : StringCompareTag
    {
        public override bool IsCondition(AbstractRequestContext context)
        {
            Object reqVal = EnsurePropertyValue(context);
            if (reqVal == null) { return false; }

            var reqValStr = reqVal is Enum ? Convert.ToInt64(reqVal).ToString() : reqVal.ToString();
            return !reqValStr.Equals(CompareValue);
        }
    }
}
