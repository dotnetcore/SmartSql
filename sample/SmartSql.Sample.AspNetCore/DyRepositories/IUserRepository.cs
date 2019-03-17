using SmartSql.DyRepository;
using SmartSql.DyRepository.Annotations;
using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartSql.Sample.AspNetCore.DyRepositories
{
    public interface IUserRepository
    {
        long Insert(User entity);
        User GetById([Param("Id")]long id);
        IEnumerable<User> Query([Param("Taken")]int taken);
        TPageResult GetByPage<TPageResult>(object request);
    }
}
