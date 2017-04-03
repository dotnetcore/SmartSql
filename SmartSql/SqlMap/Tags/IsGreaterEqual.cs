using SmartSql.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.SqlMap.Tags
{
    public class IsGreaterEqual : CompareTag
    {
        public override TagType Type => TagType.IsGreaterEqual;

        public override bool IsCondition(object paramObj)
        {
            Object reqVal = paramObj.GetValue(Property);
            bool isCondition = false;
            if (reqVal == null) { return false; }
            if (!Decimal.TryParse(reqVal.ToString(), out Decimal reqValNum)) { }
            if (!Decimal.TryParse(CompareValue, out decimal comVal)) { }
            if (reqValNum >= comVal) { return isCondition = true; }
            return isCondition;
        }
    }
}
