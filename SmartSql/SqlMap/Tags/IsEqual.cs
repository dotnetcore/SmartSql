using SmartSql.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.SqlMap.Tags
{
    public class IsEqual : CompareTag
    {
        public override TagType Type => TagType.IsEqual;

        public override bool IsCondition(object paramObj)
        {
            var reqVal = paramObj.GetValue(Property);
            if (reqVal == null) { return false; }
            return reqVal.Equals(CompareValue);
        }
    }
}
