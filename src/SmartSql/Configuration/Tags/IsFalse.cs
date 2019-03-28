using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration.Tags
{
    public class IsFalse : Tag
    {
        public override bool IsCondition(AbstractRequestContext context)
        {
            Object reqVal = EnsurePropertyValue(context);
            if (reqVal is bool val)
            {
                return val == false;
            }
            return false;
        }
    }
}
