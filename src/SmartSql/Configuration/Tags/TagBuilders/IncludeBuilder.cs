using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SmartSql.Configuration.Tags.TagBuilders
{
    public class IncludeBuilder : AbstractTagBuilder
    {
        public override ITag Build(XmlNode xmlNode, Statement statement)
        {
            var refId = GetXmlAttributeValue(xmlNode, nameof(Include.RefId));
            if (refId.IndexOf('.') < 0)
            {
                refId = $"{statement.SqlMap.Scope}.{refId}";
            }
            var includeTag = new Include
            {
                RefId = refId,
                Prepend = GetPrepend(xmlNode),
                Required = GetRequired(xmlNode),
                Statement = statement
            };
            statement.IncludeDependencies.Add(includeTag);
            return includeTag;
        }
    }
}
