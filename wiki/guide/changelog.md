---
title: Changelog
description: SmartSql version history, key milestones, and release notes. Current version 4.1.68.
---

# Changelog

SmartSql is actively maintained at [github.com/dotnetcore/SmartSql](https://github.com/dotnetcore/SmartSql). The current version is **4.1.68**, managed in `build/version.props`.

## Version 4.1.68

**Security update**: Updated `Npgsql` dependency to 4.1.13 to address [CVE security advisory](https://github.com/dotnetcore/SmartSql/commit/ebb9727).

## Key Milestones

| Milestone | Description |
|-----------|-------------|
| **Initial Release** | Core ORM with XML-managed SQL, `ISqlMapper`, middleware pipeline architecture |
| **Read/Write Splitting** | `DataSourceFilter` with weighted load balancing across read replicas ([DataSourceFilter.cs](https://github.com/dotnetcore/SmartSql/blob/master/src/SmartSql/DataSource/DataSourceFilter.cs)) |
| **Caching** | Built-in LRU and FIFO memory cache providers, Redis cache via `SmartSql.Cache.Redis` ([Cache.cs](https://github.com/dotnetcore/SmartSql/blob/master/src/SmartSql/Configuration/Cache.cs)) |
| **Dynamic Repository** | IL emit-based interface-to-implementation proxy generation ([DyRepository](https://github.com/dotnetcore/SmartSql/tree/master/src/SmartSql.DyRepository)) |
| **CUD Extensions** | Convention-based Insert/Update/Delete/GetById without XML ([CUDSqlGenerator.cs](https://github.com/dotnetcore/SmartSql/blob/master/src/SmartSql/CUD/CUDSqlGenerator.cs)) |
| **Bulk Insert** | Native bulk copy for SqlServer, MsSqlServer, MySql, MySqlConnector, and PostgreSQL |
| **Diagnostics** | `DiagnosticSource` events for command execution, session lifecycle, and errors for APM tool integration |
| **AOP Transactions** | `[Transaction]` attribute for declarative transaction management (`SmartSql.AOP`) |
| **Cache Synchronization** | Cross-instance cache invalidation via `SmartSql.Cache.Sync` with Kafka/RabbitMQ |
| **Data Synchronization** | `SmartSql.InvokeSync` with Kafka and RabbitMQ publishers for event-driven sync |
| **Property Change Tracking** | `EnablePropertyChangedTrack` for partial UPDATE statements that only modify changed fields |
| **Auto Converters** | `IAutoConverter` system for automatic parameter conversion |
| **Multiple Result Sets** | `MultipleResultMap` for paginated queries returning data + count in a single round-trip |
| **Id Generation** | Built-in SnowflakeId generator with `IdGenerator` tag support |
| **Nested Property Mapping** | Dot-notation property paths in `ResultMap` (e.g., `Prop1.Prop2.Prop3`) |
| **.NET 6 Support** | Updated to target both `netstandard2.0` and `net6.0` for test projects |

## Release History

### Recent Releases

- **4.1.68** -- Security: Updated Npgsql to 4.1.13
- **4.1.67** -- Incremented patch version
- **4.1.66** -- `IPropertyHolder` Property set to read-only; removed unused comments
- **4.1.65** -- Fixed file character encoding from GBK to UTF-8

### Core Library Stability

The core `SmartSql` package (`src/SmartSql/`) targets `netstandard2.0` and maintains backward compatibility. Extension packages are versioned independently.

## Package Versions

The version is centrally managed in [build/version.props](https://github.com/dotnetcore/SmartSql/blob/master/build/version.props):

```xml
<Project>
  <PropertyGroup>
    <Version>4.1.68</Version>
  </PropertyGroup>
</Project>
```

## Cross-References

- [Introduction](./index.md) -- Architecture overview
- [Quick Start](./quick-start.md) -- Getting started
- [Configuration](./configuration.md) -- Full configuration reference
- [XML SQL Maps](./xml-sql-maps.md) -- Dynamic SQL tag reference

## References

- [SmartSql GitHub Repository](https://github.com/dotnetcore/SmartSql)
- [build/version.props](https://github.com/dotnetcore/SmartSql/blob/master/build/version.props) -- Version definition
- [SmartSql.sln](https://github.com/dotnetcore/SmartSql/blob/master/SmartSql.sln) -- Solution file
