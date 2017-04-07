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
        public String BodyText { get; set; }
        [XmlIgnore]
        public abstract TagType Type { get; }
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
                return $" {Prepend} {BodyText}";
            }
            return String.Empty;
        }
    }
}
