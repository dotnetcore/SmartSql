using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SmartSql.Abstractions;

namespace SmartSql.Configuration.Tags
{
    public class IsNotEmpty : Tag
    {
        public override TagType Type => TagType.IsNotEmpty;

        public override bool IsCondition(RequestContext context)
        {
            Object reqVal = GetPropertyValue(context);
            if (reqVal == null)
            {
                return false;
            }
            if (reqVal is string)
            {
                return !String.IsNullOrEmpty(reqVal as string);
            }
            if (reqVal is IEnumerable)
            {
                return (reqVal as IEnumerable).GetEnumerator().MoveNext();
            }
            return reqVal.ToString().Length > 0;
        }
    }
}
