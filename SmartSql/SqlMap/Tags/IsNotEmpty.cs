using SmartSql.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.SqlMap.Tags
{
    public class IsNotEmpty : Tag
    {
        public override TagType Type => TagType.IsNotEmpty;

        public override bool IsCondition(object paramObj)
        {
            Object reqVal = paramObj.GetValue(Property);
            return ((reqVal != null) && (reqVal.ToString().Length > 0));
        }
    }
}
