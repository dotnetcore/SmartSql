using System;

namespace SmartSql.DyRepository
{
    public interface IRepositoryBuilder
    {
        Type BuildRepositoryImpl(Type interfaceType);
    }
}