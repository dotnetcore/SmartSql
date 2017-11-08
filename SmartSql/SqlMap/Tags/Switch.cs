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

        public override bool IsCondition(RequestContext context)
        {
            return true;
        }
        public override string BuildSql(RequestContext context)
        {
            var matchedTag = ChildTags.FirstOrDefault(tag =>
            {
                if (tag.Type == TagType.SwitchCase)
                {
                    var caseTag = tag as Case;
                    return caseTag.IsCondition(context);
                }
                return false;
            });
            if (matchedTag == null)
            {
                matchedTag = ChildTags.FirstOrDefault(tag => tag.Type == TagType.SwitchDefault);
            }
            if (matchedTag != null)
            {
                return matchedTag.BuildSql(context);
            }
            return String.Empty;
        }

        public class Case : CompareTag
        {
            public override TagType Type => TagType.SwitchCase;
            public override bool IsCondition(RequestContext context)
            {
                var reqVal = GetPropertyValue(context);
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

        public class Defalut : Tag
        {
            public override TagType Type => TagType.SwitchDefault;

            public override bool IsCondition(RequestContext context)
            {
                return true;
            }
        }
    }
}
