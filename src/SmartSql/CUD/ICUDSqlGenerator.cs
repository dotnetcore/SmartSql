using SmartSql.Configuration;
using System;
using System.Collections.Generic;

namespace SmartSql.CUD
{
    public interface ICUDSqlGenerator
    {

        IReadOnlyDictionary<string, Statement> StatementList { get; }

        void Init(SqlMap config, Type entityType);

        Statement BuildInsert();
        Statement BuildUpdate();

        Statement BuildDeleteMany();
        Statement BuildDeleteById();
        Statement BuildDeleteAll();

    }
}
