using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.DbSession;
using Xunit;
using Xunit.Abstractions;

namespace SmartSql.Test.Unit
{
    public class SmartSqlFixture : IDisposable
    {
        public const string GLOBAL_SMART_SQL = "GlobalSmartSql";
        public static int CtorCount = 0;
        public SmartSqlBuilder SmartSqlBuilder { get; }
        public IDbSessionFactory DbSessionFactory { get; }
        public ISqlMapper SqlMapper { get; set; }
        public SmartSqlFixture()
        {
            Console.Error.WriteLine($"SmartSqlFixture.Ctor.Invoke:{++CtorCount}.");
            SmartSqlBuilder = new SmartSqlBuilder().UseXmlConfig().UseAlias(GLOBAL_SMART_SQL).Build();
            DbSessionFactory = SmartSqlBuilder.DbSessionFactory;
            SqlMapper = SmartSqlBuilder.SqlMapper;
        }

        public void Dispose()
        {
            SmartSqlBuilder.Dispose();
        }
    }
    [CollectionDefinition(SmartSqlFixture.GLOBAL_SMART_SQL)]
    public class SmartSqlCollection : ICollectionFixture<SmartSqlFixture>
    {
    }
}
