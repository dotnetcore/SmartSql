using SmartSql.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.SqlMap.Tags
{
    public class IsNotNull : Tag
    {
        public override TagType Type => TagType.IsNotNull;

        public override bool IsCondition(object paramObj)
        {
            Object reqVal = paramObj.GetValue(Property);
            return reqVal != null;
        }
    }
}
