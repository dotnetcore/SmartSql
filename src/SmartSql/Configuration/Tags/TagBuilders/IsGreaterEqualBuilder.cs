using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SmartSql.Configuration.Tags.TagBuilders
{
    public class IsGreaterEqualBuilder : AbstractTagBuilder
    {
        public override ITag Build(XmlNode xmlNode, Statement statement)
        {
            return new IsGreaterEqual
            {
                Property = GetProperty(xmlNode),
                Prepend = GetPrepend(xmlNode),
                Required = GetRequired(xmlNode),
                CompareValue = GetCompareValueAsDecimal(xmlNode),
                ChildTags = new List<ITag>(),
                Statement = statement
            };
        }
    }
}
