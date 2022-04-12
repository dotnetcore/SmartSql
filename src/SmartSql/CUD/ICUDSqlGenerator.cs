using SmartSql.Configuration;
using System;
using System.Collections.Generic;

namespace SmartSql.CUD
{
    public interface ICUDSqlGenerator
    {

        IReadOnlyDictionary<string, Statement> StatementList { get; }

        void Generate(SqlMap config, Type entityType);

        Statement BuildInsert(GeneratorParams gParams);
        Statement BuildUpdate(GeneratorParams gParams);

        Statement BuildDeleteMany(GeneratorParams gParams);
        Statement BuildDeleteById(GeneratorParams gParams);
        Statement BuildDeleteAll(GeneratorParams gParams);

    }
}
