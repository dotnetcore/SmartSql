using System.Collections.Generic;
using SmartSql.Configuration.Maps;

namespace SmartSql.Configuration.Options
{
    public class SmartSqlConfigOptions
    {
        public string Path { get; set; }

        public Settings Settings { get; set; }

        public Database Database { get; set; }
        public IList<SmartSqlMap> SmartSqlMaps { get; set; }
        public List<TypeHandler> TypeHandlers { get; set; }
    }
}