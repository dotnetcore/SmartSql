using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Xunit;
using System.Reflection;
using System.Data.Common;

namespace SmartSql.Tests.SqlMap
{
    public class DbProvider_Test
    {
        [Fact]
        public void Load()
        {
            DbProviderFactory instance = LoadFactory();

            for (int i = 0; i < 100000; i++)
            {
                instance = LoadFactory();
            }
            Assert.NotNull(instance);
        }

        private DbProviderFactory LoadFactory()
        {
            return Assembly.Load(new AssemblyName { Name = "System.Data.SqlClient" })
                  .GetType("System.Data.SqlClient.SqlClientFactory")
                  .GetField("Instance")
                  .GetValue(null) as DbProviderFactory;
        }
    }
}
