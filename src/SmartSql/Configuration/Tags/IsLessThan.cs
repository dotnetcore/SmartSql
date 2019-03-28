using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration.Tags
{
    public class IsLessThan : NumericalCompareTag
    {
        public override bool IsCondition(AbstractRequestContext context)
        {
            Object reqVal = EnsurePropertyValue(context);
            if (reqVal == null) { return false; }
            Decimal reqValNum = 0M;
            if (reqVal is Enum)
            {
                reqValNum = Convert.ToDecimal(reqVal);
            }
            else
            {
                if (!Decimal.TryParse(reqVal.ToString(), out reqValNum)) { return false; }
            }
            return reqValNum < CompareValue;
        }
    }
}
