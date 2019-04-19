using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
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
        public ISqlMapper SqlMapper { get; }
        public ILoggerFactory LoggerFactory { get; }

        public SmartSqlFixture()
        {
            LoggerFactory = new LoggerFactory(Enumerable.Empty<ILoggerProvider>(), new LoggerFilterOptions { MinLevel = LogLevel.Debug });
            var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", $"SmartSql.log");
            LoggerFactory.AddFile(logPath, LogLevel.Trace);

            SmartSqlBuilder = new SmartSqlBuilder()
                .UseXmlConfig()
                .UseLoggerFactory(LoggerFactory)
                .UseAlias(GLOBAL_SMART_SQL)
                .Build();
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
