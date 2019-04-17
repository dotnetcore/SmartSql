using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.ConfigBuilder
{
    public enum ResourceType
    {
        File = 1,
        Directory = 2,
        DirectoryWithAllSub = 3,
        Embedded = 4,
        Uri = 5
    }
}
