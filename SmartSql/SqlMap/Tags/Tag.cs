using SmartSql.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SmartSql.SqlMap.Tags
{
    public abstract class Tag : ITag
    {
        [XmlAttribute]
        public virtual String Prepend { get; set; }
        [XmlAttribute]
        public String Property { get; set; }
        [XmlIgnore]
        public abstract TagType Type { get; }
        public IList<ITag> ChildTags { get; set; }
        public bool In { get; set; }
        public abstract bool IsCondition(object paramObj);
        public virtual String BuildSql(RequestContext context, String parameterPrefix)
        {
            if (IsCondition(context.Request))
            {
                if (In)
                {
                    return $" {Prepend} In {parameterPrefix}{Property} ";
                }

                StringBuilder strBuilder = BuildChildSql(context, parameterPrefix);
                return $" {Prepend}{strBuilder.ToString()}";
            }
            return String.Empty;
        }

        public virtual StringBuilder BuildChildSql(RequestContext context, string parameterPrefix)
        {
            StringBuilder strBuilder = new StringBuilder();
            if (ChildTags != null && ChildTags.Count > 0)
            {
                foreach (var childTag in ChildTags)
                {
                    string strSql = childTag.BuildSql(context, parameterPrefix);
                    if (String.IsNullOrWhiteSpace(strSql))
                    {
                        continue;
                    }
                    strBuilder.Append(strSql);
                }
            }

            return strBuilder;
        }
    }
}
