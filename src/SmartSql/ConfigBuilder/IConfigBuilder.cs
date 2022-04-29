using SmartSql.Configuration;
using System;

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