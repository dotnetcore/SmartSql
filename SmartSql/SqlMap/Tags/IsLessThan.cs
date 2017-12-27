using SmartSql.Abstractions;
using SmartSql.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.SqlMap.Tags
{
    public class IsLessThan : CompareTag
    {
        public override TagType Type => TagType.IsLessThan;

        public override bool IsCondition(RequestContext context)
        {
            Object reqVal = GetPropertyValue(context);
            if (reqVal == null) { return false; }
            Decimal reqValNum = 0M;
            Decimal comVal = 0M;
            if (reqVal is Enum)
            {
                reqValNum = reqVal.GetHashCode();
            }
            else
            {
                if (!Decimal.TryParse(reqVal.ToString(), out reqValNum)) { return false; }
            }
            if (!Decimal.TryParse(CompareValue, out comVal)) { return false; }
            return reqValNum < comVal;
        }
    }
}
