using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SmartSql.Configuration.Tags.TagBuilders
{
    public class ForBuilder : AbstractTagBuilder
    {
        public override ITag Build(XmlNode xmlNode, Statement statement)
        {
            var property = GetProperty(xmlNode);
            return new For
            {
                Property = property,
                Prepend = GetPrepend(xmlNode),
                Required = GetRequired(xmlNode),
                Open = GetXmlAttributeValue(xmlNode, nameof(For.Open)),
                Close = GetXmlAttributeValue(xmlNode, nameof(For.Close)),
                Separator = GetXmlAttributeValue(xmlNode, nameof(For.Separator)),
                Key = GetXmlAttributeValue(xmlNode, nameof(For.Key)) ?? property,
                ChildTags = new List<ITag>(),
                Statement = statement
            };
        }
    }
}