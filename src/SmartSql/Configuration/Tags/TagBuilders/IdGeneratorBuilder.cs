using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using SmartSql.Exceptions;
using SmartSql.IdGenerator;

namespace SmartSql.Configuration.Tags.TagBuilders
{
    public class IdGeneratorBuilder : AbstractTagBuilder
    {
        public override ITag Build(XmlNode xmlNode, Statement statement)
        {
            IIdGenerator idGen = statement.SqlMap.SmartSqlConfig.IdGenerators.Values.FirstOrDefault();
            var idGenName = GetXmlAttributeValue(xmlNode, "Name");
            if (!String.IsNullOrEmpty(idGenName))
            {
                if (!statement.SqlMap.SmartSqlConfig.IdGenerators.TryGetValue(idGenName, out idGen))
                {
                    throw new SmartSqlException($"Can not find IdGenerator.Name:{idGenName},XmlNode:{xmlNode}.");
                }
            }

            if (!xmlNode.Attributes.TryGetValueAsBoolean(nameof(IdGenerator.Assign), out var assign))
            {
                assign = true;
            }

            return new IdGenerator
            {
                Statement = statement,
                Id = GetXmlAttributeValue(xmlNode, nameof(IdGenerator.Id)),
                IdGen = idGen,
                Assign = assign
            };
        }
    }
}