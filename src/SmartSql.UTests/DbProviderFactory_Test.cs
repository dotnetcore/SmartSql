using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Text;
using Xunit;
namespace SmartSql.UTests
{
    public class DbProviderFactory_Test : TestBase
    {
        [Fact]
        public void Load()
        {
            Assert.NotNull(DbProviderFactory);
        }
    }
}
