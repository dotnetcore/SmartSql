using SmartSql.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.SqlMap.Tags
{
    public class IsLessEqual : CompareTag
    {
        public override TagType Type => TagType.IsLessEqual;

        public override bool IsCondition(object paramObj)
        {
            Object reqVal = paramObj.GetValue(Property);
            bool isCondition = false;
            if (reqVal == null) { return false;  }
            if (!Decimal.TryParse(reqVal.ToString(), out Decimal reqValNum)) { };
            if (!Decimal.TryParse(CompareValue, out decimal comVal)) { }
            if (reqValNum <= comVal) { isCondition = true; }
            return isCondition;
        }
    }
}
