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
            return new For
            {
                Property = GetProperty(xmlNode),
                Prepend = GetPrepend(xmlNode),
                Open = GetXmlAttributeValue(xmlNode, nameof(For.Open)),
                Close = GetXmlAttributeValue(xmlNode, nameof(For.Close)),
                Separator = GetXmlAttributeValue(xmlNode, nameof(For.Separator)),
                Key = GetXmlAttributeValue(xmlNode, nameof(For.Key)),
                ChildTags = new List<ITag>(),
                Statement = statement
            };
        }
    }
}
