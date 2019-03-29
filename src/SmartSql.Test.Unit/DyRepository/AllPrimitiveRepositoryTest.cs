using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Test.Repositories;
using Xunit;

namespace SmartSql.Test.Unit.DyRepository
{
    public class AllPrimitiveRepositoryTest : DyRepositoryTest
    {
        [Fact]
        public void Build()
        {
            var smartSqlBuilder = new SmartSqlBuilder().UseXmlConfig().Build();
            var repositoryType = RepositoryBuilder.Build(typeof(IAllPrimitiveRepository), smartSqlBuilder.SmartSqlConfig);
            Assert.NotNull(repositoryType);
        }
    }
}
