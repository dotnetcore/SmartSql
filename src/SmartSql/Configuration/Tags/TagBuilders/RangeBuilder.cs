using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SmartSql.Configuration.Tags.TagBuilders
{
    public class RangeBuilder : AbstractTagBuilder
    {
        public override ITag Build(XmlNode xmlNode, Statement statement)
        {
            return new Range
            {
                Property = GetProperty(xmlNode),
                Prepend = GetPrepend(xmlNode),
                Required = GetRequired(xmlNode),
                Min = GetXmlAttributeValueAsDecimal(xmlNode, nameof(Range.Min)),
                Max = GetXmlAttributeValueAsDecimal(xmlNode, nameof(Range.Max)),
                ChildTags = new List<ITag>(),
                Statement = statement
            };
        }
    }
}
