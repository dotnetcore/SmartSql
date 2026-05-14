# Engineering Quality Optimization Design

## Overview

Refactor SmartSql's test infrastructure and CI/CD pipeline to improve engineering quality. Key changes: split tests into pure unit tests and integration tests, upgrade to .NET 8.0, upgrade CI Actions, and add code coverage.

## 1. Project Structure Changes

**Current**: `src/SmartSql.Test.Unit` — all tests mixed together, depends on MySQL.

**After**:

```
src/SmartSql.Test.Unit/          # Pure unit tests, zero external dependencies, Moq + xUnit
src/SmartSql.Test.Integration/   # Integration tests, requires MySQL/Redis
```

### Steps

1. Rename `SmartSql.Test.Unit` to `SmartSql.Test.Integration` (project file, assembly name, namespace → `SmartSql.Test.Integration`)
2. Create new `SmartSql.Test.Unit` project, target `net8.0`, reference xUnit, Moq, FluentAssertions, coverlet.collector
3. Migrate database-independent tests from Integration to Unit
4. `SmartSqlFixture`, `AbstractTest`, and all SqlMapper-dependent tests stay in Integration

## 2. CI Pipeline Design

### build.yml (new)

```yaml
Trigger: push / pull_request to master
Jobs:
  build-and-test:
    runs-on: ubuntu-latest
    steps:
      - checkout@v4
      - setup-dotnet@v4 (net8.0.x)
      - dotnet build SmartSql.sln
      - dotnet test src/SmartSql.Test.Unit (coverlet coverage)
      - ReportGenerator → HTML report
      - Upload to Codecov
```

- No service containers, fast execution (target < 2 min)

### integration-test.yml (upgrade)

```yaml
Trigger: push / pull_request to master
Jobs:
  integration-test:
    runs-on: ubuntu-latest
    services: MySQL (system) + Redis (container)
    steps:
      - checkout@v4
      - setup-dotnet@v4 (net8.0.x)
      - init-mysql-db
      - dotnet test src/SmartSql.Test.Integration
```

### package-publish.yml (upgrade)

- Upgrade `checkout@v4`, `setup-dotnet@v4`, `net8.0.x`
- Otherwise unchanged

### Pipeline relationship

`build.yml` and `integration-test.yml` run in parallel. PR merge requires both to pass.

## 3. Testing Standards

### Unit Test Project (`SmartSql.Test.Unit`)

**Dependencies**: xUnit (latest), Moq, FluentAssertions, coverlet.collector

**Conventions**:
- Naming: `{ClassUnderTest}Tests.cs`, method names `MethodName_Scenario_ExpectedBehavior`
- One test class per production class
- `[Fact]` for single tests, `[Theory]` + `[InlineData]` for parameterized tests
- Mock boundary dependencies (IDbSession, IDbCommand, IDataReader etc.), never mock the class under test
- No file system, network, or database dependencies

### Integration Test Project (`SmartSql.Test.Integration`)

**Keep existing patterns**: `SmartSqlFixture` + `[Collection]` shared SqlMapper instance.

**Adjustments**:
- Namespace → `SmartSql.Test.Integration`
- `EnvironmentFactAttribute` stays in Integration project
- Upgrade xUnit and related packages to match Unit project versions

### Shared test helpers

`SmartSql.Test` (entities, repositories) referenced only by Integration. Unit project references only source projects under test.

## 4. Dependency Upgrades

### Target frameworks

| Project | Current | After |
|---------|---------|-------|
| SmartSql (core) | netstandard2.0 | **unchanged** — maximize compatibility |
| SmartSql.Test.Unit | net6.0 | net8.0 |
| SmartSql.Test.Integration | net6.0 | net8.0 |

### Package upgrades

| Package | Current | Target |
|---------|---------|--------|
| xUnit | 2.4.1 | 2.9.x |
| xunit.runner.visualstudio | 2.4.3 | 2.8.x |
| Microsoft.NET.Test.Sdk | 16.11.0 | 17.x |
| actions/checkout | master | v4 |
| actions/setup-dotnet | v2 | v4 |

### New packages

| Package | Purpose |
|---------|---------|
| Moq | Unit project mocking |
| coverlet.collector | Coverage data collection |
| FluentAssertions | Readable assertions |

## 5. Migration Priority

### Batch 1 — Pure logic (no risk)

| Module | Files | Description |
|--------|-------|-------------|
| Reflection/ | 10 | Object construction / reflection |
| FlexibleConvert/ | 8 | Type conversion |
| Utils/ | 5 | Utility classes |
| Cryptos/ | 3 | Encryption / decryption |
| Attempts/ | 2 | Pure logic |

### Batch 2 — Requires light mocking

| Module | Files | Description |
|--------|-------|-------------|
| Deserializer/ | 8 | Mock IDataReader |
| TypeHandlers/ | 5 | Mock DbDataReader |
| Tags/ (partial) | TBD | SQL build logic mockable |
| ConfigBuilder/ | 2 | XML config parsing |

### Batch 3 — Stays in Integration

| Module | Reason |
|--------|--------|
| DbSessions/ | Real DB connection |
| DyRepository/ | Dynamic proxy + SqlMapper |
| CUD/ | Real CRUD |
| Cache/ | Redis cache |
| Bulk/ | DB-specific bulk operations |
| DI/ | DI container integration |
| SmartSqlBuilderTest | Full build pipeline |
| ErrorDiagnosis/ | Full pipeline diagnostics |

## 6. Execution Order

1. Create new `SmartSql.Test.Unit` project + CI `build.yml`
2. Rename old project to `SmartSql.Test.Integration` + upgrade `integration-test.yml`
3. Migrate Batch 1 tests
4. Migrate Batch 2 tests
5. Set up coverlet + ReportGenerator + Codecov
6. Upgrade `package-publish.yml`
