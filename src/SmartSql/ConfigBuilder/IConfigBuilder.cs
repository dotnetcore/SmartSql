using SmartSql.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.ConfigBuilder
{
    public interface IConfigBuilder : IDisposable
    {
        bool Initialized { get; }
        SmartSqlConfig SmartSqlConfig { get; }
        IConfigBuilder Parent { get; }
        SmartSqlConfig Build();
        void SetParent(IConfigBuilder configBuilder);
    }
}