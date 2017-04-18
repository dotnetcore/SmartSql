using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Common;
using System.Xml.Serialization;
using System.Linq;

namespace SmartSql.SqlMap.Tags
{
    public class Switch : ITag
    {
        public TagType Type => TagType.Switch;
        [XmlAttribute]
        public String Prepend { get; set; }
        [XmlAttribute]
        public String Property { get; set; }
        public IList<Case> Cases { get; set; }
        public string BuildSql(object paramObj, string parameterPrefix)
        {
            Object reqVal = paramObj.GetValue(Property);
            if (reqVal == null) { return ""; }
            String valStr = reqVal.ToString();
            var caseVal = Cases.FirstOrDefault(m => m.CompareValue == valStr);
            if (caseVal == null) { return ""; }
            return $" {Prepend} {caseVal.BodyText}";
        }
        public class Case : ITag
        {
            public String CompareValue { get; set; }
            public String BodyText { get; set; }
            public TagType Type => TagType.Case;

            public string BuildSql(object paramObj, string parameterPrefix)
            {
                return BodyText;
            }
        }
    }
}
