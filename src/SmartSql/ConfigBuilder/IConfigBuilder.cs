using SmartSql.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.ConfigBuilder
{
    public interface IConfigBuilder : IDisposable
    {
        SmartSqlConfig Build(IEnumerable<KeyValuePair<string, string>> importProperties);
    }
}
