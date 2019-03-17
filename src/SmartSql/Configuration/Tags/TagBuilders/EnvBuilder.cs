using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SmartSql.Configuration.Tags.TagBuilders
{
    public class EnvBuilder : AbstractTagBuilder
    {
        public override ITag Build(XmlNode xmlNode, Statement statement)
        {
            return new Env
            {
                Prepend = GetPrepend(xmlNode),
                DbProvider = GetXmlAttributeValue(xmlNode, "DbProvider"),
                ChildTags = new List<ITag>(),
                Statement = statement
            };
        }
    }
}
