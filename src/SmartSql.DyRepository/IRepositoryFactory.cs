using SmartSql.Abstractions;
using System;

namespace SmartSql.DyRepository
{
    public interface IRepositoryFactory
    {
        object CreateInstance(Type interfaceType, ISmartSqlMapper smartSqlMapper);
        T CreateInstance<T>(ISmartSqlMapper smartSqlMapper);
    }
}