using SmartSql.Configuration;
using SmartSql.DbSession;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Test.Unit
{
    public abstract class AbstractXmlConfigBuilderTest : IDisposable
    {
        protected IDbSessionFactory DbSessionFactory { get; }
        protected SmartSqlBuilder SmartSqlBuilder { get; }
        public IDbSession DbSession { get; }
        public AbstractXmlConfigBuilderTest()
        {
            SmartSqlBuilder = SmartSqlBuilder.AddXmlConfig().Build();
            DbSessionFactory = SmartSqlBuilder.GetDbSessionFactory();
            DbSession = DbSessionFactory.Open();
        }
        public void Dispose()
        {
            DbSession.Dispose();
        }
    }
}
