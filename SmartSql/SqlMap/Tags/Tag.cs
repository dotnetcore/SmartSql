using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SmartSql.SqlMap.Tags
{
    public abstract class Tag : ITag
    {
        [XmlAttribute]
        public String Prepend { get; set; }
        [XmlAttribute]
        public String Property { get; set; }
        [XmlIgnore]
        public abstract TagType Type { get; }
        public IList<ITag> ChildTags { get; set; }
        public bool In { get; set; }
        public abstract bool IsCondition(object paramObj);
        public virtual String BuildSql(object paramObj, String parameterPrefix)
        {
            if (IsCondition(paramObj))
            {
                if (In)
                {
                    return $" {Prepend} In {parameterPrefix}{Property}";
                }
                StringBuilder strBuilder = new StringBuilder();
                if (ChildTags != null && ChildTags.Count > 0)
                {
                    foreach (var childTag in ChildTags)
                    {
                        string strSql = childTag.BuildSql(paramObj, parameterPrefix);
                        strBuilder.Append(strSql);
                    }
                }
                return $" {Prepend} {strBuilder.ToString()}";
            }
            return String.Empty;
        }
    }
}
