using System;

namespace SmartSql.Configuration.Tags
{
    public class IsTrue : Tag
    {
        public override bool IsCondition(RequestContext context)
        {
            Object reqVal = GetPropertyValue(context);
            if (reqVal is Boolean)
            {
                return (bool)reqVal == true;
            }
            return false;
        }
    }
}
