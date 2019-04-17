using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SmartSql.Configuration.Tags.TagBuilders
{
    public class SetBuilder : AbstractTagBuilder
    {
        public const int DEFAULT_MIN = 1;
        public override ITag Build(XmlNode xmlNode, Statement statement)
        {
            var setTag = new Set
            {
                ChildTags = new List<ITag>(),
                Required = GetRequired(xmlNode),
                Min = DEFAULT_MIN,
                Statement = statement
            };
            var min = GetMin(xmlNode);
            if (min.HasValue)
            {
                setTag.Min = min;
            }
            return setTag;
        }
    }
}
