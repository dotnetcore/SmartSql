using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Test.Repositories
{
    public interface IUserRepository
    {
        long Insert(User user);
        IEnumerable<User> Query();
    }
}
