using SmartSql.Abstractions;
using System;

namespace SmartSql.DyRespository
{
    public interface IRespositoryFactory
    {
        object CreateInstance(Type interfaceType, ISmartSqlMapper smartSqlMapper);
        T CreateInstance<T>(ISmartSqlMapper smartSqlMapper);
    }
}