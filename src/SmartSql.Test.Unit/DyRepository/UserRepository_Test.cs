using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Data;
using SmartSql.DyRepository;
using SmartSql.Test.Repositories;
using Xunit;
using Microsoft.Extensions.Logging;

namespace SmartSql.Test.Unit.DyRepository
{
    [Collection("GlobalSmartSql")]
    public class UserRepository_Test
    {
        private IUserRepository _userRepository;
        public UserRepository_Test(SmartSqlFixture smartSqlFixture)
        {
            _userRepository = smartSqlFixture.UserRepository;
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
