using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SmartSql.Configuration.Tags.TagBuilders
{
    public class SwitchCaseBuilder : AbstractTagBuilder
    {
        public override ITag Build(XmlNode xmlNode, Statement statement)
        {
            var switchNode = xmlNode.ParentNode;
            return new Switch.Case
            {
                Property = GetProperty(switchNode),
                Prepend = GetPrepend(switchNode),
                CompareValue = GetCompareValue(xmlNode),
                ChildTags = new List<ITag>(),
                Statement = statement
            };
        }
    }
}
