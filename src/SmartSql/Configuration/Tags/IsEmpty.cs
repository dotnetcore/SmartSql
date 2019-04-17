using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Collections;

namespace SmartSql.Configuration.Tags
{
    public class IsEmpty : Tag
    {
        public override bool IsCondition(AbstractRequestContext context)
        {
            Object reqVal = EnsurePropertyValue(context);
            switch (reqVal)
            {
                case null:
                    return true;
                case string reqStr:
                    return String.IsNullOrEmpty(reqStr);
                case IEnumerable reqEnum:
                    return !reqEnum.GetEnumerator().MoveNext();
                default:
                    return reqVal.ToString().Length == 0;
            }
        }
    }
}
