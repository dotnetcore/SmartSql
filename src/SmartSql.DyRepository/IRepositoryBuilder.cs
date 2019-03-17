using SmartSql.Configuration;
using SmartSql.DbSession;
using System;

namespace SmartSql.DyRepository
{
    public interface IRepositoryBuilder
    {
        Type Build(Type interfaceType, SmartSqlConfig smartSqlConfig, string scope = "");
    }
}
