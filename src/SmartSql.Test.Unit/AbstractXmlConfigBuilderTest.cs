using SmartSql.Configuration;
using SmartSql.DbSession;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Test.Unit
{
    public abstract class AbstractXmlConfigBuilderTest
    {
        protected ISqlMapper BuildSqlMapper(string alias)
        {
            lock (this)
            {
                var smartsqlBuilder = SmartSqlContainer.Instance.GetSmartSql(alias);
                return smartsqlBuilder != null ? smartsqlBuilder.SqlMapper : new SmartSqlBuilder().UseXmlConfig().UseAlias(alias).Build().SqlMapper;

            }
        }

    }
}
