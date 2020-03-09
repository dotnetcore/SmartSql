using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace SmartSql.DataSource
{
    public class DbProvider
    {
        public const String SQLSERVER = "SqlServer";
        public const String MS_SQLSERVER = "MsSqlServer";
        public const String MYSQL = "MySql";
        public const String MYSQL_CONNECTOR = "MySqlConnector";
        
        public const String POSTGRESQL = "PostgreSql";
        public const String ORACLE = "Oracle";
        public const String SQLITE = "SQLite";

        public String Name { get; set; }
        public String ParameterNamePrefix { get; internal set; }
        public String ParameterNameSuffix { get; internal set; }
        public String SelectAutoIncrement { get; set; }
        public String ParameterPrefix { get; set; }
        public String Type { get; set; }
        public DbProviderFactory Factory { get; set; }
    }
}
