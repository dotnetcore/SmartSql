---
title: DataSource & Read/Write Splitting
description: How SmartSql manages database connections and automatically routes queries to read replicas versus the write master.
---

# DataSource & Read/Write Splitting

SmartSql provides built-in support for read/write splitting, allowing you to direct read queries to one or more read replicas while routing write operations to a single master database. This feature is essential for scaling read-heavy applications and is configured through XML or programmatic APIs. The routing decision happens inside the middleware pipeline via `DataSourceFilterMiddleware`, which delegates to an `IDataSourceFilter` implementation.

## At a Glance

| Aspect | Detail |
|--------|--------|
| Abstract base | `AbstractDataSource` with Name, ConnectionString, DbProvider |
| Write source | `WriteDataSource` -- single master database |
| Read source | `ReadDataSource` -- one or more replicas with a `Weight` property |
| Filter interface | `IDataSourceFilter` with `Elect(AbstractRequestContext)` |
| Default filter | `DataSourceFilter` with weighted load balancing via `WeightFilter<T>` |
| Selection logic | Statement type (`Read`/`Write`) determines source choice |
| Extension point | Replace `IDataSourceFilter` via `SmartSqlBuilder.UseDataSourceFilter()` |

## DataSource Class Hierarchy

```mermaid
classDiagram
    class AbstractDataSource {
        <<abstract>>
        +String Name
        +String ConnectionString
        +DbProvider DbProvider
        +CreateConnection() DbConnection
    }
    class WriteDataSource {
    }
    class ReadDataSource {
        +int Weight
    }
    class Database {
        +DbProvider DbProvider
        +WriteDataSource Write
        +IDictionary~String, ReadDataSource~ Reads
    }
    class DataSourceChoice {
        <<enumeration>>
        Unknow
        Write
        Read
    }

    AbstractDataSource <|-- WriteDataSource
    AbstractDataSource <|-- ReadDataSource
    Database --> WriteDataSource
    Database --> ReadDataSource
    Database --> DbProvider
```

<!-- Sources: src/SmartSql/DataSource/AbstractDataSource.cs:9, src/SmartSql/DataSource/WriteDataSource.cs:7, src/SmartSql/DataSource/ReadDataSource.cs:7, src/SmartSql/DataSource/Database.cs:7 -->

## How Data Source Selection Works

When the middleware pipeline reaches `DataSourceFilterMiddleware`, it delegates to `IDataSourceFilter.Elect()` to determine which database connection to use.

```mermaid
sequenceDiagram
autonumber
    participant DSM as DataSourceFilterMiddleware
    participant DSF as DataSourceFilter
    participant SS as SessionStore
    participant DB as Database
    participant WF as WeightFilter

    DSM->>DSM: Check if session already has DataSource
    alt Session already has DataSource
        DSM-->>DSM: Reuse existing (e.g., transaction context)
    else No DataSource assigned
        DSM->>DSF: Elect(requestContext)
        DSF->>SS: Check LocalSession.DataSource
        alt LocalSession exists with DataSource
            DSF-->>DSM: Return existing DataSource
        else No existing DataSource
            DSF->>DB: Read requestContext.DataSourceChoice
            alt Choice == Write
                DSF-->>DSM: Return Database.Write
            else Choice == Read
                alt ReadDb specified
                    DSF->>DB: Reads[ReadDb]
                    DSF-->>DSM: Return specific ReadDataSource
                else No ReadDb specified
                    DSF->>WF: Elect(readSources with weights)
                    WF-->>DSF: Selected ReadDataSource
                    DSF-->>DSM: Return elected read source
                end
            end
        end
    end
    DSM->>DSM: dbSession.SetDataSource(electedSource)
```

<!-- Sources: src/SmartSql/Middlewares/DataSourceFilterMiddleware.cs:7, src/SmartSql/DataSource/DataSourceFilter.cs:11, src/SmartSql/DataSource/DataSourceFilter.cs:24 -->

## DataSourceChoice Determination

The `DataSourceChoice` (Read or Write) is determined during the `InitializerMiddleware` phase based on the statement's `StatementType`:

| StatementType Mapping | Resulting Choice |
|----------------------|-----------------|
| `StatementType.Write` (or contains Write flag) | `DataSourceChoice.Write` |
| All other types (Select, etc.) | `DataSourceChoice.Read` |
| Explicit `SourceChoice` on the Statement | Overrides automatic detection |

```mermaid
flowchart TD
    subgraph Decision["DataSourceChoice Decision"]
        style Decision fill:#161b22,stroke:#30363d,color:#e6edf3
        Start["Request has Statement"] --> Check1{"Statement.SourceChoice<br>explicitly set?"}
        Check1 -->|Yes| Use["Use explicit SourceChoice"]
        Check1 -->|No| Check2{"StatementType<br>contains Write?"}
        Check2 -->|Yes| Write["DataSourceChoice.Write"]
        Check2 -->|No| Read["DataSourceChoice.Read"]
        Write --> Result["Set on requestContext"]
        Read --> Result
        Use --> Result
    end

    style Start fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style Check1 fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style Check2 fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style Use fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style Write fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style Read fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style Result fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
```

<!-- Sources: src/SmartSql/Middlewares/InitializerMiddleware.cs:64, src/SmartSql/DataSource/DataSourceChoice.cs:7 -->

## Weighted Load Balancing for Read Replicas

When multiple read replicas are configured and no specific `ReadDb` is requested, `DataSourceFilter` uses `WeightFilter<T>` to perform weighted random selection. Each `ReadDataSource` has a `Weight` property that influences the probability of selection.

| Replica | Weight | Selection Probability |
|---------|--------|----------------------|
| Read-1 | 100 | 50% |
| Read-2 | 60 | 30% |
| Read-3 | 40 | 20% |

This allows you to direct more traffic to more powerful replicas while still distributing load.

## XML Configuration

Database sources are configured in the `SmartSqlMapConfig.xml` file:

```xml
<SmartSqlMapConfig>
  <Database>
    <DbProvider Name="MySql"/>
    <Write Name="WriteDB"
           ConnectionString="Server=master-db;Database=MyDb;Uid=root;Pwd=123456;"/>
    <Read Name="ReadDB-1" Weight="100"
          ConnectionString="Server=replica-1;Database=MyDb;Uid=readonly;Pwd=123456;"/>
    <Read Name="ReadDB-2" Weight="60"
          ConnectionString="Server=replica-2;Database=MyDb;Uid=readonly;Pwd=123456;"/>
  </Database>
  <SmartSqlMaps>
    <SmartSqlMap Resource="Maps/User.xml"/>
  </SmartSqlMaps>
</SmartSqlMapConfig>
```

## Programmatic Configuration

When using `SmartSqlBuilder`, you can configure the data source directly without XML:

```csharp
// Single database (no read/write splitting)
new SmartSqlBuilder()
    .UseDataSource("MySql", connectionString)
    .Build();

// Or with a WriteDataSource object
new SmartSqlBuilder()
    .UseDataSource(new WriteDataSource
    {
        Name = "Write",
        ConnectionString = masterConnectionString,
        DbProvider = dbProvider
    })
    .Build();
```

## Explicit ReadDb Selection

Individual statements or request contexts can specify a `ReadDb` property to target a specific read replica, bypassing the weighted selection:

```xml
<Statement Id="GetReport" ReadDb="ReadDB-1">
  SELECT * FROM Reports WHERE Id = @Id
</Statement>
```

## Transaction Behavior

When a transaction is active (`IDbSession.Transaction != null`), the `DataSourceFilter` always returns the data source already assigned to the session. This ensures all operations within a transaction hit the same database connection, regardless of read/write designation.

## IDataSourceFilter Interface

```csharp
public interface IDataSourceFilter
{
    AbstractDataSource Elect(AbstractRequestContext context);
}
```

To implement custom routing logic (e.g., based on tenant, region, or latency), create a class implementing `IDataSourceFilter` and register it:

```csharp
new SmartSqlBuilder()
    .UseDataSourceFilter(new MyCustomDataSourceFilter())
    .Build();
```

<!-- Sources: src/SmartSql/DataSource/IDataSourceFilter.cs:11, src/SmartSql/DataSource/DataSourceFilter.cs:11 -->

## Connection Creation

`AbstractDataSource.CreateConnection()` uses the `DbProvider.Factory` to create a new `DbConnection` instance and assigns the `ConnectionString`:

```mermaid
flowchart LR
    subgraph Creation["Connection Creation"]
        style Creation fill:#161b22,stroke:#30363d,color:#e6edf3
        DS["AbstractDataSource"] --> F["DbProvider.Factory"]
        F --> Conn["Factory.CreateConnection()"]
        Conn --> CS["conn.ConnectionString = this.ConnectionString"]
        CS --> Return["Return DbConnection"]
    end

    style DS fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style F fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style Conn fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style CS fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style Return fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
```

<!-- Sources: src/SmartSql/DataSource/AbstractDataSource.cs:23 -->

## Cross-References

- [Architecture Overview](./index.md) -- where DataSource fits in the layered architecture
- [Middleware Pipeline](./middleware-pipeline.md) -- `DataSourceFilterMiddleware` at Order 400
- [Caching Architecture](./caching.md) -- cache behavior differences in transaction context

## References

- [AbstractDataSource.cs](https://github.com/dotnetcore/SmartSql/blob/master/src/SmartSql/DataSource/AbstractDataSource.cs)
- [WriteDataSource.cs](https://github.com/dotnetcore/SmartSql/blob/master/src/SmartSql/DataSource/WriteDataSource.cs)
- [ReadDataSource.cs](https://github.com/dotnetcore/SmartSql/blob/master/src/SmartSql/DataSource/ReadDataSource.cs)
- [Database.cs](https://github.com/dotnetcore/SmartSql/blob/master/src/SmartSql/DataSource/Database.cs)
- [DataSourceFilter.cs](https://github.com/dotnetcore/SmartSql/blob/master/src/SmartSql/DataSource/DataSourceFilter.cs)
- [IDataSourceFilter.cs](https://github.com/dotnetcore/SmartSql/blob/master/src/SmartSql/DataSource/IDataSourceFilter.cs)
- [DataSourceChoice.cs](https://github.com/dotnetcore/SmartSql/blob/master/src/SmartSql/DataSource/DataSourceChoice.cs)
- [DataSourceFilterMiddleware.cs](https://github.com/dotnetcore/SmartSql/blob/master/src/SmartSql/Middlewares/DataSourceFilterMiddleware.cs)
