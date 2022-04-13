using SmartSql.Test.Entities;
using System.Collections.Generic;
using System.Data;
using SmartSql.Data;
using SmartSql.DyRepository.Annotations;

namespace SmartSql.Test.Repositories
{
    public interface IUserRepository
    {
        long Insert(User user);
        IEnumerable<User> Query();

        [Statement(CommandType = CommandType.StoredProcedure, Sql = "SP_Query")]
        IEnumerable<AllPrimitive> SP_Query(SqlParameterCollection sqlParameterCollection);
    }
}