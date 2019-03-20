using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Microsoft.Extensions.Configuration
{
    public static class IConfigurationExtensions
    {
        public static IDictionary<string, string> AsProperties(this IConfiguration configuration)
        {
            return configuration.AsEnumerable().ToDictionary((kv) => kv.Key, (kv) => kv.Value);
        }
    }
}
