using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SmartSql.Configuration.Tags.TagBuilders
{
    public class PlaceholderBuilder : AbstractTagBuilder
    {
        public override ITag Build(XmlNode xmlNode, Statement statement)
        {
            return new Placeholder
            {
                Property = GetProperty(xmlNode),
                Required = GetRequired(xmlNode),
                Prepend = GetPrepend(xmlNode),
                ChildTags = new List<ITag>(),
                Statement = statement
            };
        }
    }
}
