using SmartSql.Configuration;
using SmartSql.DataSource;
using System;
using System.Collections.Generic;

namespace SmartSql.Options
{
    public class SmartSqlConfigOptions
    {
        public Settings Settings { get; set; } = Settings.Default;
        public IDictionary<String, string> Properties { get; set; } = new Dictionary<string, string>();
        public Database Database { get; set; }
        public List<TypeHandler> TypeHandlers { get; set; } = new List<TypeHandler>();
        public List<TagBuilder> TagBuilders { get; set; } = new List<TagBuilder>();
        public List<IdGenerator> IdGenerators { get; set; }
        public List<SqlMapSource> SmartSqlMaps { get; set; } = new List<SqlMapSource>();

        public List<AutoConverterBuilder> AutoConverterBuilders { get; set; }
    }
}
