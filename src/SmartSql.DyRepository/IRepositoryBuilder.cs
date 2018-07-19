using SmartSql.Abstractions;
using System;

namespace SmartSql.DyRepository
{
    public interface IRepositoryBuilder
    {
        Type BuildRepositoryImpl(Type interfaceType, ISmartSqlMapper smartSqlMapper, string scope = "");
    }
}