using System.Collections.Generic;
using System.Xml;

namespace SmartSql.Configuration.Tags.TagBuilders
{
    public class NowBuilder : AbstractTagBuilder
    {
        public override ITag Build(XmlNode xmlNode, Statement statement)
        {
            return new Now
            {
                Property = GetProperty(xmlNode),
                Prepend = GetPrepend(xmlNode),
                Required = GetRequired(xmlNode),
                Kind = GetXmlAttributeValue(xmlNode, nameof(Now.Kind)),
                ChildTags = new List<ITag>(),
                Statement = statement
            };
        }
    }
}