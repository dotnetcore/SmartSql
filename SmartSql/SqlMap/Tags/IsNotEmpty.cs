using SmartSql.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SmartSql.SqlMap.Tags
{
    public class IsNotEmpty : Tag
    {
        public override TagType Type => TagType.IsNotEmpty;

        public override bool IsCondition(object paramObj)
        {
            Object reqVal = paramObj.GetValue(Property);
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
