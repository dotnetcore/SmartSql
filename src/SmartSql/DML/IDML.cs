using System;
using System.Data;

namespace SmartSql.DML
{
    public interface IDML
    {
        StatementType StatementType { get; }
        String Operation { get;  }
        String Table { get; set; }

    }
}