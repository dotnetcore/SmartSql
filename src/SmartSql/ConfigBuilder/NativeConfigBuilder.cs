using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Configuration;

namespace SmartSql.ConfigBuilder
{
    public class NativeConfigBuilder : IConfigBuilder
    {
        public NativeConfigBuilder(SmartSqlConfig smartSqlConfig)
        {
            SmartSqlConfig = smartSqlConfig;
        }

        public void Dispose()
        {
            
        }

        public bool Initialized { get; } = true;
        public SmartSqlConfig SmartSqlConfig { get; }
        public IConfigBuilder Parent { get; }

        public SmartSqlConfig Build()
        {
            return SmartSqlConfig;
        }

        public void SetParent(IConfigBuilder configBuilder)
        {
           
        }
    }
}