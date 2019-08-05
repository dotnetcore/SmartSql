using System;
using SmartSql.Configuration;

namespace SmartSql.DML
{
    public interface IStatement
    {
        StatementType StatementType { get; }
        String Operation { get;  }
        String Table { get; set; }
    }
}