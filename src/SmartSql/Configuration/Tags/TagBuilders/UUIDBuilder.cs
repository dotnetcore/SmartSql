using System.Collections.Generic;
using System.Xml;

namespace SmartSql.Configuration.Tags.TagBuilders
{
    public class UUIDBuilder : AbstractTagBuilder
    {
        public override ITag Build(XmlNode xmlNode, Statement statement)
        {
            return new UUID
            {
                Property = GetProperty(xmlNode),
                Prepend = GetPrepend(xmlNode),
                Required = GetRequired(xmlNode),
                Format = GetXmlAttributeValue(xmlNode, nameof(UUID.Format)),
                ChildTags = new List<ITag>(),
                Statement = statement
            };
        }
    }
}