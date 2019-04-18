using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Data;
using SmartSql.Test.Repositories;
using Xunit;

namespace SmartSql.Test.Unit.DyRepository
{
    public class UserRepository_Test : DyRepositoryTest
    {
        private IUserRepository _userRepository;
        public UserRepository_Test()
        {
            var smartSqlBuilder = new SmartSqlBuilder().UseXmlConfig().UseAlias(this.GetType().FullName).Build();
            _userRepository = RepositoryFactory.CreateInstance(typeof(IUserRepository), smartSqlBuilder.SqlMapper) as IUserRepository;
        }


        [Fact]
        public void SP_QueryUser()
        {
            SqlParameterCollection dbParameterCollection = new SqlParameterCollection();
            dbParameterCollection.Add(new SqlParameter("Total", null, typeof(int))
            {
                DbType = System.Data.DbType.Int32,
                Direction = System.Data.ParameterDirection.Output
            });
            var list = _userRepository.SP_QueryUser(dbParameterCollection);
            Assert.NotNull(list);
            dbParameterCollection.TryGetParameterValue("Total", out int total);
            Assert.NotEqual(0, total);
        }
    }
}
