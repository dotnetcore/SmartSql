using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace SmartSql.SqlMap.Tags
{
    public class IsProperty : Tag
    {
        public override TagType Type => TagType.IsProperty;

        public override bool IsCondition(object paramObj)
        {
            return paramObj.GetType().GetProperty(Property) != null;
        }
    }
}
