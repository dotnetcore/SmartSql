using SmartSql.DbSession;
using System;

namespace SmartSql.DyRepository
{
    public interface IRepositoryFactory
    {
        object CreateInstance(Type interfaceType, ISqlMapper sqlMapper, string scope = "");
    }
}
