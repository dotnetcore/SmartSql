using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SmartSql.Configuration.Tags.TagBuilders
{
    public class DynamicBuilder : AbstractTagBuilder
    {
        public override ITag Build(XmlNode xmlNode, Statement statement)
        {
            return new Dynamic
            {
                Prepend = GetPrepend(xmlNode),
                Required = GetRequired(xmlNode),
                Min = GetMin(xmlNode),
                Statement = statement,
                ChildTags = new List<ITag>()
            };
        }
    }
}
