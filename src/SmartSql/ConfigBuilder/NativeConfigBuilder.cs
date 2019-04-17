using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Configuration;

namespace SmartSql.ConfigBuilder
{
    public class NativeConfigBuilder : IConfigBuilder
    {
        private readonly SmartSqlConfig _smartSqlConfig;

        public NativeConfigBuilder(SmartSqlConfig smartSqlConfig)
        {
            _smartSqlConfig = smartSqlConfig;
        }

        public SmartSqlConfig Build(IEnumerable<KeyValuePair<string, string>> importProperties)
        {
            ImportProperties(importProperties);
            return _smartSqlConfig;
        }

        private void ImportProperties(IEnumerable<KeyValuePair<string, string>> importProperties)
        {
            if (importProperties != null) { _smartSqlConfig.Properties.Import(importProperties); }
        }

        public void Dispose()
        {

        }
    }
}
