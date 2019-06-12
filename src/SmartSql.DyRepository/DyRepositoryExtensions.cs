using System;

namespace SmartSql.DyRepository
{
    public static class DyRepositoryExtensions
    {
        public static bool IsDyRepository(this Type repositoryInterface)
        {
            return typeof(IRepositoryProxy).IsAssignableFrom(repositoryInterface);
        }
        public static bool IsDyRepository(this object repositoryObj)
        {
            return repositoryObj is IRepositoryProxy;
        }
    }
}