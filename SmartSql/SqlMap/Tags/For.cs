using SmartSql.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
namespace SmartSql.SqlMap.Tags
{
    public class For : Tag
    {
        public const string FOR_KEY_SUFFIX = "_For_";
        public override TagType Type => TagType.For;
        public string Open { get; set; }
        public string Separator { get; set; }
        public string Close { get; set; }
        public string Key { get; set; }

        public override bool IsCondition(object paramObj)
        {
            var reqVal = paramObj.GetValue(Property);
            if (reqVal == null) { return false; }
            if (reqVal is IEnumerable) { return true; }
            return false;
        }

        public override string BuildSql(RequestContext context, string parameterPrefix)
        {
            if (IsCondition(context.RequestParameters))
            {
                return BuildChildSql(context, parameterPrefix).ToString();
            }
            return String.Empty;
        }
        public override StringBuilder BuildChildSql(RequestContext context, string parameterPrefix)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.AppendFormat(" {0}", Prepend);
            strBuilder.Append(Open);
            int item_index = 0;

            var reqVal = context.RequestParameters.Get<IEnumerable>(Property);
            //** 目前仅支持子标签为SqlText **
            var bodyText = (ChildTags[0] as SqlText).BodyText;
            foreach (var itemVal in reqVal)
            {
                string key_name = $"{parameterPrefix}{Key}{FOR_KEY_SUFFIX}{item_index}";
                context.RequestParameters.Add(key_name, itemVal);
                if (item_index > 0)
                {
                    strBuilder.AppendFormat(" {0} ", Separator);
                }
                string item_sql = Regex.Replace(bodyText
                                  , ("([?@:]" + Regex.Escape(Key) + @")(?!\w)(\s+(?i)unknown(?-i))?")
                                  , key_name
                                  , RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant);

                strBuilder.AppendFormat("{0}", item_sql);
                item_index++;
            }
            strBuilder.Append(Close);
            return strBuilder;
        }
    }
}
