using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SmartSql.Configuration;
using SmartSql.Data;
using SmartSql.DyRepository.Annotations;

namespace SmartSql.Test.Repositories
{
    public interface IUserRepository
    {
        long Insert(User user);
        IEnumerable<User> Query();
        [Statement(CommandType = CommandType.StoredProcedure,Sql = "SP_QueryUser")]
        IEnumerable<User> SP_QueryUser(SqlParameterCollection sqlParameterCollection);
    }
}
