using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SmartSql.Configuration.Tags.TagBuilders
{
    public class WhereBuilder : AbstractTagBuilder
    {
        public override ITag Build(XmlNode xmlNode, Statement statement)
        {
            return new Where
            {
                ChildTags = new List<ITag>(),
                Required = GetRequired(xmlNode),
                Min = GetMin(xmlNode),
                Statement = statement
            };
        }
    }
}
