using SmartSql.Configuration;
using SmartSql.DataSource;
using System;
using System.Collections.Generic;

namespace SmartSql.Options
{
    public class SmartSqlConfigOptions
    {
        public Settings Settings { get; set; }
        public Database Database { get; set; }
        public Properties Properties { get; set; }
        ///public List<SmartSqlMapSource> SmartSqlMaps { get; set; }
        public List<TypeHandler> TypeHandlers { get; set; }
    }
}
