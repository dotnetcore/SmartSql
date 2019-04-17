using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SmartSql.Configuration.Tags
{
    public class IsNotEmpty : Tag
    {
        public override bool IsCondition(AbstractRequestContext context)
        {
            Object reqVal = EnsurePropertyValue(context);
            switch (reqVal)
            {
                case null:
                    return false;
                case string reqStr:
                    return !String.IsNullOrEmpty(reqStr);
                case IEnumerable reqEnum:
                    return reqEnum.GetEnumerator().MoveNext();
                default:
                    return reqVal.ToString().Length > 0;
            }
        }
    }
}
