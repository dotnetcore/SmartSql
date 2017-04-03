using SmartSql.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.SqlMap.Tags
{
    public class IsNotEqual : CompareTag
    {
        public override TagType Type => TagType.IsNotEqual;

        public override bool IsCondition(object paramObj)
        {
            var reqVal = paramObj.GetValue(Property);
            if (reqVal == null) { return false; }
            return !reqVal.Equals(CompareValue);

        }
    }
}
