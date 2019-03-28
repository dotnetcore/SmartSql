using System;

namespace SmartSql.Configuration.Tags
{
    public class IsTrue : Tag
    {
        public override bool IsCondition(AbstractRequestContext context)
        {
            Object reqVal = EnsurePropertyValue(context);
            if (reqVal is bool val)
            {
                return val == true;
            }
            return false;
        }
    }
}
