using System;
using System.Collections.Generic;
using System.Xml;

namespace SmartSql.Configuration.Tags.TagBuilders
{
    public class OrderByBuilder: AbstractTagBuilder
    {
        public override ITag Build(XmlNode xmlNode, Statement statement)
        {
            return new OrderBy
            {
                Property = GetProperty(xmlNode),
                Required = GetRequired(xmlNode),
                ChildTags = new List<ITag>(),
                Statement = statement
            };
        }
    }
}