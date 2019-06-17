using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SmartSql.Configuration.Tags.TagBuilders
{
    public class IsNotPropertyBuilder : AbstractTagBuilder
    {
        public override ITag Build(XmlNode xmlNode, Statement statement)
        {
            return new IsNotProperty
            {
                Property = GetProperty(xmlNode),
                Prepend = GetPrepend(xmlNode),
                ChildTags = new List<ITag>(),
                Statement = statement
            };
        }
    }
}