using SmartSql.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.SqlMap.Tags
{
    public class IsFalse : Tag
    {
        public override TagType Type => TagType.IsFalse;

        public override bool IsCondition(object paramObj)
        {
            Object reqVal = paramObj.GetValue(Property);
            if (reqVal is Boolean)
            {
                return (bool)reqVal == false;
            }
            return false;
        }
    }
}
