using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SmartSql.Configuration.Tags.TagBuilders
{
    public class IdGeneratorBuilder : AbstractTagBuilder
    {
        public override ITag Build(XmlNode xmlNode, Statement statement)
        {
            return new IdGenerator
            {
                Statement = statement,
                Id = GetXmlAttributeValue(xmlNode, nameof(IdGenerator.Id)),
                IdGen = statement.SqlMap.SmartSqlConfig.IdGenerator
            };
        }
    }
}
