using System.Collections.Generic;
using SmartSql.Configuration;

namespace SmartSql.Options
{
    public class SmartSqlConfigOptions
    {
        public Settings Settings { get; set; }

        public Database Database { get; set; }

        public List<SmartSqlMapSource> SmartSqlMaps { get; set; }

        public List<TypeHandler> TypeHandlers { get; set; }
    }
}