# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

SmartSql is a .NET ORM library inspired by MyBatis. It uses XML to manage SQL statements, supports read/write splitting, caching (memory & Redis), dynamic repository proxies, bulk inserts, AOP transactions, and diagnostics. Version is managed in `build/version.props`.

Core library targets `netstandard2.0` (C# 7.3). Test projects target `net8.0`. All projects use the FluentAssertions library for assertions.

## Build & Test Commands

```bash
# Build the entire solution
dotnet build SmartSql.sln

# Run all unit tests (SQLite in-memory, no external dependencies)
dotnet test src/SmartSql.Test.Unit/SmartSql.Test.Unit.csproj

# Run a specific unit test by name
dotnet test src/SmartSql.Test.Unit/SmartSql.Test.Unit.csproj --filter "FullyQualifiedName~SmartSql.Test.Unit.Tests.CacheTest"

# Run integration tests (requires Docker for Testcontainers: MySQL 8.0 + Redis 7)
dotnet test src/SmartSql.Test.Integration/SmartSql.Test.Integration.csproj

# Pack NuGet packages (Release)
dotnet pack -c Release -o ./nuget
```

Tests use xUnit. There are two test projects:
- **`SmartSql.Test.Unit`** — Pure unit tests using SQLite in-memory (`UseDataSource(DbProvider.SQLITE, "Data Source=:memory:")`). No external services needed.
- **`SmartSql.Test.Integration`** — Integration tests using Testcontainers (MySQL 8.0 + Redis 7). Requires Docker. The `SmartSqlFixture` (xUnit `IAsyncLifetime`) starts containers, initializes the database from `DB/init-mysql-db.sql`, and registers repositories.

Both test projects reference **`SmartSql.Test`** which contains shared entities, DTOs, and repository interfaces used across tests.

## Architecture

### Key Abstractions

- **`ISqlMapper`** / **`SqlMapper`** (`src/SmartSql/SqlMapper.cs`) — The main entry point. Provides sync/async methods: `Execute`, `ExecuteScalar<T>`, `Query<T>`, `QuerySingle<T>`, `GetDataTable`, `GetDataSet`. Wraps `IDbSession` with automatic session lifecycle management.

- **`SmartSqlBuilder`** (`src/SmartSql/SmartSqlBuilder.cs`) — Fluent builder that constructs the entire runtime. Chain methods: `UseXmlConfig()`, `UseDataSource()`, `UseCache()`, `RegisterEntity()`, `AddTypeHandler()`, `AddFilter()`, `AddMiddleware()`, etc. Registers the built instance into `SmartSqlContainer`.

- **`SmartSqlConfig`** (`src/SmartSql/Configuration/SmartSqlConfig.cs`) — Central configuration holding Database, SqlMaps, Pipeline, CacheManager, TypeHandlerFactory, Filters, IdGenerators, and all resolved settings.

- **`ExecutionContext`** — Carries `SmartSqlConfig`, `IDbSession`, `AbstractRequestContext`, and `ResultContext` through the pipeline.

### Middleware Pipeline

All SQL execution flows through a linked-list middleware pipeline (`IMiddleware`). Default order (by `IOrdered.Order`):

1. **InitializerMiddleware** — Resolves the Statement from config, sets up the request context
2. **PrepareStatementMiddleware** — Builds the SQL string from Statement tags
3. **CachingMiddleware** — (when cache enabled) Checks/populates cache
4. **TransactionMiddleware** — Manages DB transactions
5. **DataSourceFilterMiddleware** — Selects read vs write data source
6. **CommandExecuterMiddleware** — Executes the DbCommand
7. **ResultHandlerMiddleware** — Deserializes results via `IDataReaderDeserializer`

The pipeline is built by `PipelineBuilder` which sorts middlewares by `Order` and chains them via `Next` pointers.

### XML SQL Management

SQL statements are defined in XML files (SmartSqlMap). Each XML file maps to a `SqlMap` (identified by `Scope`) containing `Statement` elements. Statements use dynamic tags from `src/SmartSql/Configuration/Tags/` for conditional SQL construction (e.g., `IsNotEmpty`, `IsEqual`, `Switch`, `Where`, `Set`, `For`, `Include`, `Env`).

### Extension Projects

| Project | Purpose |
|---------|---------|
| `SmartSql.DyRepository` | Dynamic proxy repository generation via IL emit. Interface methods auto-map to SQL statements by naming convention. |
| `SmartSql.DIExtension` | ASP.NET Core DI integration (`services.AddSmartSql()`) |
| `SmartSql.Options` | Options-pattern config builder (for `appsettings.json`) |
| `SmartSql.Cache.Redis` | Redis cache provider |
| `SmartSql.Cache.Sync` | Cache synchronization |
| `SmartSql.TypeHandler` | JSON and other custom type handlers |
| `SmartSql.TypeHandler.PostgreSql` | PostgreSQL-specific type handlers |
| `SmartSql.AOP` | AOP transaction support (`[Transaction]` attribute) |
| `SmartSql.Extensions` | General extensions |
| `SmartSql.ScriptTag` | Script tag support for dynamic SQL |
| `SmartSql.Bulk.*` | Bulk insert providers (SqlServer, MsSqlServer, MySql, MySqlConnector, PostgreSql) |
| `SmartSql.InvokeSync` + Kafka/RabbitMQ | Data synchronization via message queues |
| `SmartSql.Oracle` | Oracle DB provider support |
| `SmartSql.DataConnector` | Data connector service |

### Deserialization Chain

DataReader deserializers are tried in order via `DeserializerFactory`:
`MultipleResultDeserializer` → `ValueTupleDeserializer` → `ValueTypeDeserializer` → `DynamicDeserializer` → `EntityDeserializer` → custom deserializers

### Diagnostics

`src/SmartSql/Diagnostics/` emits `DiagnosticSource` events for command execution, session lifecycle, and errors — enabling integration with APM tools like SkyWalking.
