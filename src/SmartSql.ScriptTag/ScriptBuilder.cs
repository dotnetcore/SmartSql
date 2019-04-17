using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using SmartSql.Configuration;
using SmartSql.Configuration.Tags;
using SmartSql.Configuration.Tags.TagBuilders;

namespace SmartSql.ScriptTag
{
    public class ScriptBuilder : AbstractTagBuilder
    {
        public override ITag Build(XmlNode xmlNode, Statement statement)
        {
            if (!xmlNode.Attributes.TryGetValueAsString(nameof(Script.Test), out var test))
            {
                throw new ArgumentNullException(nameof(Script.Test));
            }
            return new Script(test)
            {
                Prepend = GetPrepend(xmlNode),
                Property = GetProperty(xmlNode),
                Statement = statement,
                ChildTags = new List<ITag>()
            };
        }
    }
}
