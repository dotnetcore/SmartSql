using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SmartSql.Configuration.Tags.TagBuilders
{
    public class SqlTextBuilder : ITagBuilder
    {
        public ITag Build(XmlNode xmlNode, Statement statement)
        {
            var innerText = xmlNode.InnerText;
            var bodyText = innerText.Replace(statement.SqlMap.SmartSqlConfig.Settings.ParameterPrefix
                , statement.SqlMap.SmartSqlConfig.Database.DbProvider.ParameterPrefix);
            return new SqlText(bodyText
                , statement.SqlMap.SmartSqlConfig.Database.DbProvider.ParameterPrefix)
            {
                Statement = statement
            };
        }
    }
}
