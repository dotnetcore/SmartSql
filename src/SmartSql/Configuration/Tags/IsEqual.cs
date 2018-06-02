using SmartSql.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration.Tags
{
    public class IsEqual : CompareTag
    {
        public override TagType Type => TagType.IsEqual;

        public override bool IsCondition(RequestContext context)
        {
            var reqVal = GetPropertyValue(context);
            if (reqVal == null) { return false; }
            string reqValStr = string.Empty;
            if (reqVal is Enum)
            {
                reqValStr = reqVal.GetHashCode().ToString();
            }
            else
            {
                reqValStr = reqVal.ToString();
            }
            return reqValStr.Equals(CompareValue);
        }
    }
}
