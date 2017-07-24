using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Common;
using System.Xml.Serialization;
using System.Linq;
using SmartSql.Abstractions;

namespace SmartSql.SqlMap.Tags
{
    public class Switch : Tag
    {
        public override TagType Type => TagType.Switch;

        public override bool IsCondition(object paramObj)
        {
            return true;
        }

        public class Case : CompareTag
        {
            public override TagType Type => TagType.Case;
            public override bool IsCondition(object paramObj)
            {
                var reqVal = paramObj.GetValue(Property);
                if (reqVal == null) { return false; }
                string reqValStr = string.Empty;
                if (reqVal is Enum)
                {
                    reqValStr = reqVal.GetHashCode().ToString();
                }
                else
                {
                    reqValStr = reqVal.ToString();
                }
                return reqValStr.Equals(CompareValue);
            }
        }
    }
}
