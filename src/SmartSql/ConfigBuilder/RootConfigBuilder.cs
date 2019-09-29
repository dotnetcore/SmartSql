using System.Collections.Generic;
using SmartSql.Configuration;

namespace SmartSql.ConfigBuilder
{
    public class RootConfigBuilder : IConfigBuilder
    {
        public bool Initialized { get; }
        public SmartSqlConfig SmartSqlConfig { get; }
        public IConfigBuilder Parent { get; }

        public RootConfigBuilder() : this(null)
        {
        }

        public RootConfigBuilder(IEnumerable<KeyValuePair<string, string>> importProperties)
        {
            SmartSqlConfig = new SmartSqlConfig();
            if (importProperties != null)
            {
                SmartSqlConfig.Properties.Import(importProperties);
            }
            Initialized = true;
        }

        public void Dispose()
        {
        }

        public SmartSqlConfig Build()
        {
            return SmartSqlConfig;
        }

        public void SetParent(IConfigBuilder configBuilder)
        {
        }
    }
}