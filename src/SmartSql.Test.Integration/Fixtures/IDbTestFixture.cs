using Microsoft.Extensions.Logging;
using SmartSql.DyRepository;
using SmartSql.Test.Repositories;
using Xunit;

namespace SmartSql.Test.Integration.Fixtures;

public interface IDbTestFixture : IAsyncLifetime
{
    ISqlMapper SqlMapper { get; }
    SmartSqlBuilder SmartSqlBuilder { get; }
    string DbProvider { get; }
    ILoggerFactory LoggerFactory { get; }
    IRepositoryFactory RepositoryFactory { get; }
    IAllPrimitiveRepository AllPrimitiveRepository { get; }
    IUserRepository UserRepository { get; }
}
