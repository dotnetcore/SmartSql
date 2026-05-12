# AGENTS.md — SmartSql

## Build & Run Commands

```bash
# Build entire solution
dotnet build SmartSql.sln

# Run all tests
dotnet test

# Run specific test project
dotnet test src/SmartSql.Test.Unit/SmartSql.Test.Unit.csproj

# Run specific test by name
dotnet test src/SmartSql.Test.Unit/SmartSql.Test.Unit.csproj --filter "FullyQualifiedName~SmartSql.Test.Unit.Tests.CacheTest"

# Pack NuGet packages (Release)
dotnet pack -c Release -o ./nuget
```

## Testing

- **Framework**: xUnit
- **Shared fixture**: `SmartSqlFixture` in `src/SmartSql.Test.Unit/SmartSqlFixture.cs`
- **Collection**: `[Collection("GlobalSmartSql")]` for tests using the shared fixture
- **Conditional tests**: `EnvironmentFactAttribute` skips tests based on env vars (e.g., `REDIS`, `GITHUB_ACTIONS`)
- **Performance**: BenchmarkDotNet in `src/SmartSql.Test.Performance/`
- **Requirements**: MySQL database, optionally Redis

## Project Structure

```
SmartSql.sln
├── src/
│   ├── SmartSql/                    # Core ORM library (netstandard2.0)
│   │   ├── Configuration/           # Config model (SmartSqlConfig, SqlMap, Statement)
│   │   ├── Configuration/Tags/      # Dynamic XML SQL tags
│   │   ├── Middlewares/             # Middleware pipeline
│   │   ├── DbSession/               # Database session management
│   │   ├── DataSource/              # Read/write data source management
│   │   ├── Cache/                   # Built-in cache (LRU, FIFO)
│   │   ├── Deserializer/            # DataReader → Object deserialization
│   │   ├── Diagnostics/             # DiagnosticSource events
│   │   ├── Filters/                 # Filter interfaces
│   │   ├── IdGenerator/             # SnowflakeId, DbSequence
│   │   ├── TypeHandlers/            # Type handler framework
│   │   └── Reflection/              # IL emit utilities
│   ├── SmartSql.DyRepository/       # Dynamic repository proxy (IL emit)
│   ├── SmartSql.DIExtension/        # ASP.NET Core DI integration
│   ├── SmartSql.Options/            # Options pattern (appsettings.json)
│   ├── SmartSql.AOP/                # AOP transaction (AspectCore)
│   ├── SmartSql.Cache.Redis/        # Redis cache provider
│   ├── SmartSql.Cache.Sync/         # Distributed cache sync
│   ├── SmartSql.TypeHandler/        # JSON/XML/Crypto type handlers
│   ├── SmartSql.TypeHandler.PostgreSql/  # PostgreSQL type handlers
│   ├── SmartSql.Bulk*/              # Bulk insert (5 DB variants)
│   ├── SmartSql.InvokeSync*/        # Kafka/RabbitMQ data sync
│   ├── SmartSql.Oracle/             # Oracle support
│   ├── SmartSql.ScriptTag/          # Script tag (Jint JS engine)
│   ├── SmartSql.DataConnector/      # Cross-DB replication service
│   ├── SmartSql.Extensions/         # General extensions
│   ├── SmartSql.Test.Unit/          # Unit tests
│   ├── SmartSql.Test.Performance/   # Benchmarks
│   └── SmartSql.Test/               # Shared test library
├── sample/
│   └── SmartSql.Sample.AspNetCore/  # Sample ASP.NET Core app
├── build/
│   └── version.props                # Version: 4.1.68
└── wiki/                            # VitePress documentation site
```

## Code Style

- **Language**: C# 7.3, targeting `netstandard2.0` for core
- **Naming**: PascalCase for types/methods, camelCase for locals/parameters
- **Patterns**: Builder pattern (SmartSqlBuilder), Middleware pipeline (IMiddleware linked list), Factory pattern (DeserializerFactory, TagBuilderFactory)
- **Interfaces**: Prefixed with `I` (ISqlMapper, IMiddleware, ICache)
- **Extensions**: Named `{Feature}Extensions` (SmartSqlDIExtensions, BulkExtensions)

## Git Workflow

- **Main branch**: `master`
- **Version management**: Edit `build/version.props` (VersionMajor, VersionMinor, VersionPatch)
- **CI**: GitHub Actions — integration-test.yml (on push), package-publish.yml (on release)
- **NuGet**: Published on GitHub release creation

## Boundaries

- Do NOT modify core library (`src/SmartSql/`) without running full test suite
- Do NOT change middleware order without verifying pipeline tests
- XML config changes require both test XML and sample XML updates
- Extension projects should remain independent (no cross-references between extensions)
- Wiki content: see `wiki/AGENTS.md`
