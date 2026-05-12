---
title: Oracle Support
description: Oracle database provider support with OracleCommand-specific configuration
---

# Oracle Support

While SmartSql's core can work with any ADO.NET provider, Oracle's `OracleCommand` requires specific configuration that deviates from the standard ADO.NET behavior. The `SmartSql.Oracle` package provides a custom `CommandExecuter` that configures Oracle-specific properties (`BindByName`, `InitialLONGFetchSize`) on every command before execution.

## At a Glance

| Feature | Description |
|---------|-------------|
| Package | `SmartSql.Oracle` |
| Provider | Oracle.ManagedDataAccess (ODP.NET) |
| Key Feature | Auto-sets `BindByName = true` and `InitialLONGFetchSize = -1` |
| Entry Point | `SmartSqlBuilder.UseOracleCommandExecuter()` |

## Why Oracle Needs Special Handling

By default, Oracle's ODP.NET driver binds parameters by position rather than by name. This conflicts with SmartSql's XML-based parameter binding, which uses named parameters (`:Name`, `:Age`, etc.). Setting `BindByName = true` on every `OracleCommand` fixes this, and the `SmartSql.Oracle` package automates that configuration.

```mermaid
flowchart TD
    subgraph Problem["Without Oracle Extension"]
        style Problem fill:#161b22,stroke:#30363d,color:#e6edf3
        A1["XML: SELECT * FROM Users WHERE Name = :Name"] --> A2["OracleCommand created"]
        A2 --> A3["BindByName = false (default)"]
        A3 --> A4["Parameter binding by position<br>= WRONG RESULTS"]
    end

    subgraph Solution["With Oracle Extension"]
        style Solution fill:#161b22,stroke:#30363d,color:#e6edf3
        B1["XML: SELECT * FROM Users WHERE Name = :Name"] --> B2["OracleCommand created"]
        B2 --> B3["Event: DbCommandCreated"]
        B3 --> B4["BindByName = true<br>InitialLONGFetchSize = -1"]
        B4 --> B5["Parameter binding by name<br>= CORRECT"]
    end

    style A1 fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style A2 fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style A3 fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style A4 fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style B1 fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style B2 fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style B3 fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style B4 fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style B5 fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
```

<!-- Sources: src/SmartSql.Oracle/SmartSqlBuilderExtensions.cs:10, src/SmartSql.Oracle/SmartSqlBuilderExtensions.cs:24 -->

## How It Works

The extension hooks into the `DbCommandCreated` event of the `CommandExecuter`. Every time a new `DbCommand` is created, the handler checks if it is an `OracleCommand` and configures the required properties:

```mermaid
sequenceDiagram
autonumber
    participant Builder as SmartSqlBuilder
    participant CE as CommandExecuter
    participant Event as DbCommandCreated Event
    participant Handler as OnDbCommandCreated
    participant OC as OracleCommand

    Builder->>Builder: UseOracleCommandExecuter()
    Builder->>CE: new CommandExecuter()
    Builder->>CE: Subscribe DbCommandCreated

    Note over CE: During SQL execution
    CE->>OC: new OracleCommand()
    CE->>Event: Fire DbCommandCreated
    Event->>Handler: OnDbCommandCreated(sender, args)
    Handler->>OC: Cast to OracleCommand
    alt Not OracleCommand
        Handler->>Handler: throw SmartSqlException
    end
    Handler->>OC: BindByName = true
    Handler->>OC: InitialLONGFetchSize = -1
    Handler-->>CE: Continue execution
```

<!-- Sources: src/SmartSql.Oracle/SmartSqlBuilderExtensions.cs:24 -->

## Configuration Properties Set

| Property | Value | Purpose |
|---|---|---|
| `BindByName` | `true` | Enables named parameter binding (essential for SmartSql XML maps) |
| `InitialLONGFetchSize` | `-1` | Fetches entire LONG/LONG RAW column values (Oracle default is 0, which returns only the length) |

## Setup

### Basic Registration

```csharp
var smartSqlBuilder = new SmartSqlBuilder()
    .UseOracleCommandExecuter()
    .UseXmlConfig(ResourceType.File, "SmartSqlMapConfig.xml")
    .Build();
```

### With DI Integration

```csharp
services.AddSmartSql((sp, builder) =>
{
    builder
        .UseProperties(Configuration)
        .UseOracleCommandExecuter(sp.GetService<ILoggerFactory>());
});
```

### With Custom LoggerFactory

```csharp
builder.UseOracleCommandExecuter(loggerFactory);
```

## Integration Architecture

```mermaid
graph LR
    subgraph OracleSetup["Oracle Integration Setup"]
        style OracleSetup fill:#161b22,stroke:#30363d,color:#e6edf3
        Builder["SmartSqlBuilder"] --> Ext["UseOracleCommandExecuter()"]
        Ext --> CE["CommandExecuter"]
        CE --> Hook["DbCommandCreated event"]
        Hook --> OC["OracleCommand Config"]
        OC --> BN["BindByName = true"]
        OC --> LF["InitialLONGFetchSize = -1"]
    end

    subgraph Runtime["Runtime"]
        style Runtime fill:#161b22,stroke:#30363d,color:#e6edf3
        Pipeline["Middleware Pipeline"] --> CE2["CommandExecuter"]
        CE2 --> OC2["OracleCommand"]
        OC2 --> BN2["Parameters bound by name"]
        BN2 --> DB["Oracle Database"]
    end

    style Builder fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style Ext fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style CE fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style Hook fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style OC fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style BN fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style LF fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style Pipeline fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style CE2 fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style OC2 fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style BN2 fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style DB fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
```

<!-- Sources: src/SmartSql.Oracle/SmartSqlBuilderExtensions.cs:10, src/SmartSql.Oracle/SmartSqlBuilderExtensions.cs:24 -->

Both overloads accept an optional `ILoggerFactory`. The parameterless version uses the `SmartSqlBuilder.LoggerFactory` that was already configured.

## XML Configuration for Oracle

Your SmartSql XML configuration should specify the Oracle database provider:

```xml
<SmartSqlMapConfig>
  <Database>
    <DbProvider Name="Oracle" ParameterPrefix=":"/>
    <Write Name="Write" ConnectionString="Data Source=...;User Id=...;Password=...;"/>
    <Reads>
      <Read Name="Read" ConnectionString="Data Source=...;User Id=...;Password=...;" Weight="100"/>
    </Reads>
  </Database>
  <SmartSqlMaps>
    <SmartSqlMap Path="Maps" Type="Directory"/>
  </SmartSqlMaps>
</SmartSqlMapConfig>
```

::: warning
Oracle's parameter prefix is `:`, not `@` (SqlServer) or `?` (MySql). Make sure your XML maps use the correct prefix, or configure `ParameterPrefix` in your database provider settings.
:::

## Error Handling

If the `DbCommandCreated` event fires but the command is not an `OracleCommand` (for example, if the wrong ADO.NET provider is configured), the handler throws a `SmartSqlException`:

```
The ADO.NET Driver is not [Oracle.ManagedDataAccess.Core].
```

This ensures fail-fast behavior when the Oracle extension is registered but the wrong database provider is in use.

## API Reference

### SmartSqlBuilderExtensions (Oracle)

| Method | Description |
|---|---|
| `UseOracleCommandExecuter(SmartSqlBuilder)` | Register Oracle command executer using the builder's logger factory |
| `UseOracleCommandExecuter(SmartSqlBuilder, ILoggerFactory)` | Register Oracle command executer with an explicit logger factory |

## Cross-References

- **[DI Integration](./di-extension.md)** -- Combine Oracle support with ASP.NET Core DI.
- **[Configuration (XML)](../guide/configuration.md)** -- XML database provider configuration.
- **[Bulk Insert](./bulk-insert.md)** -- Note: there is no Oracle-specific bulk insert provider; use SmartSql's standard insert mechanisms for Oracle.

## References

- [SmartSqlBuilderExtensions.cs](https://github.com/dotnetcore/SmartSql/blob/master/src/SmartSql.Oracle/SmartSqlBuilderExtensions.cs) -- Full implementation with event hook
- [SmartSqlBuilder.cs](https://github.com/dotnetcore/SmartSql/blob/master/src/SmartSql/SmartSqlBuilder.cs) -- Central builder (UseCommandExecuter method)
- [CommandExecuter.cs](https://github.com/dotnetcore/SmartSql/blob/master/src/SmartSql/Command/CommandExecuter.cs) -- Base command executer with DbCommandCreated event
- [SmartSqlConfig.cs](https://github.com/dotnetcore/SmartSql/blob/master/src/SmartSql/Configuration/SmartSqlConfig.cs) -- Configuration holding database provider settings
- [DbProvider.cs](https://github.com/dotnetcore/SmartSql/blob/master/src/SmartSql/DataSource/DbProvider.cs) -- Database provider abstraction
