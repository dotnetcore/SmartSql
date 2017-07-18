using SmartSql.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.SqlMap.Tags
{
    public class IsGreaterThan : CompareTag
    {
        public override TagType Type => TagType.IsGreaterThan;

        public override bool IsCondition(object paramObj)
        {
            Object reqVal = paramObj.GetValue(Property);
            if (reqVal == null) { return false; }

            Decimal reqValNum = 0M;
            Decimal comVal = 0M;
            if (reqVal is Enum)
            {
                reqValNum = (Decimal)reqVal;
            }
            else
            {
                if (!Decimal.TryParse(reqVal.ToString(), out reqValNum)) { return false; }
            }
            if (!Decimal.TryParse(CompareValue, out comVal)) { return false; }
            return reqValNum > comVal;
        }
    }
}
