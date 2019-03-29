using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.DyRepository;

namespace SmartSql.Test.Unit.DyRepository
{
    public class DyRepositoryTest
    {
        protected IRepositoryBuilder  RepositoryBuilder { get; set; }
        protected IRepositoryFactory RepositoryFactory { get; set; }
        public DyRepositoryTest()
        {
            RepositoryBuilder = new EmitRepositoryBuilder(null, null, Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance);
            RepositoryFactory = new RepositoryFactory(RepositoryBuilder, Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance);
        }
    }
}
