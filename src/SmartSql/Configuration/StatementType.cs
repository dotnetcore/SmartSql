using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration
{
    [Flags]
    public enum StatementType
    {
        Unknown = 0,
        Insert = 1 << 0,
        Update = 1 << 1,
        Delete = 1 << 2,
        Select = 1 << 3,
        Write = Insert | Update | Delete
    }
}
