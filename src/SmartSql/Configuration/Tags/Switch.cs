using System;
using System.Linq;

namespace SmartSql.Configuration.Tags
{
    public class Switch : Tag
    {
        public override bool IsCondition(RequestContext context)
        {
            return true;
        }
        public override void BuildSql(RequestContext context)
        {
            var matchedTag = ChildTags.FirstOrDefault(tag =>
            {
                if (tag is Case)
                {
                    var caseTag = tag as Case;
                    return caseTag.IsCondition(context);
                }
                return false;
            });
            if (matchedTag == null)
            {
                matchedTag = ChildTags.FirstOrDefault(tag => tag is Default);
            }
            if (matchedTag != null)
            {
                matchedTag.BuildSql(context);
            }

        }

        public class Case : StringCompareTag
        {
            public override bool IsCondition(RequestContext context)
            {
                var reqVal = GetPropertyValue(context);
                if (reqVal == null) { return false; }
                string reqValStr = string.Empty;
                if (reqVal is Enum)
                {
                    reqValStr = Convert.ToInt64(reqVal).ToString();
                }
                else
                {
                    reqValStr = reqVal.ToString();
                }
                return reqValStr.Equals(CompareValue);
            }
        }

        public class Default : Tag
        {
            public override bool IsCondition(RequestContext context)
            {
                return true;
            }
        }
    }
}
