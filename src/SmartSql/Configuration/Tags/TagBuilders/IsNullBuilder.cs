using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SmartSql.Configuration.Tags.TagBuilders
{
    public class IsNullBuilder : AbstractTagBuilder
    {
        public override ITag Build(XmlNode xmlNode, Statement statement)
        {
            return new IsNull
            {
                Property = GetProperty(xmlNode),
                Prepend = GetPrepend(xmlNode),
                Required = GetRequired(xmlNode),
                ChildTags = new List<ITag>(),
                Statement = statement
            };
        }
    }
}
