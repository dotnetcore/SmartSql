using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using SmartSql.Common;
namespace SmartSql.SqlMap.Tags
{
    public class IsEmpty : Tag
    {
        public override TagType Type => TagType.IsEmpty;

        public override bool IsCondition(object paramObj)
        {
            Object reqVal = paramObj.GetValue(Property);
            return !((reqVal != null) && (reqVal.ToString().Length > 0));
        }
    }
}
