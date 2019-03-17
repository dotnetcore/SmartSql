using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration.Tags
{
    public class IsFalse : Tag
    {
        public override bool IsCondition(RequestContext context)
        {
            Object reqVal = GetPropertyValue(context);
            if (reqVal is Boolean)
            {
                return (bool)reqVal == false;
            }
            return false;
        }
    }
}
