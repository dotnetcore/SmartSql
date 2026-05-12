---
title: Bulk Insert
description: High-performance bulk data loading using database-specific native APIs
---

# Bulk Insert

Inserting large volumes of data one row at a time is prohibitively slow for most production workloads. The `SmartSql.Bulk` package provides a database-agnostic interface for high-performance bulk inserts, with native implementations for SQL Server, MySQL, MySQL (MySqlConnector), and PostgreSQL. Each implementation uses the database's own bulk loading mechanism -- `SqlBulkCopy`, `MySqlBulkLoader`, and `COPY BINARY` respectively -- to achieve maximum throughput.

## At a Glance

| Feature | Description |
|---------|-------------|
| Package | `SmartSql.Bulk` (base) |
| Implementations | SqlServer, MsSqlServer, MySql, MySqlConnector, PostgreSql |
| Input | `DataTable` or `IEnumerable<TEntity>` |
| Interface | `IBulkInsert` |
| Sync/Async | `Insert()` and `InsertAsync()` |

## Class Hierarchy

```mermaid
classDiagram
    class IBulkInsert {
        <<interface>>
        +DataTable Table
        +Insert()
        +InsertAsync()
        +Dispose()
    }

    class AbstractBulkInsert {
        <<abstract>>
        +DataTable Table
        +IDbSession DbSession
        +Insert()*
        +InsertAsync()*
        +Dispose()
    }

    class SqlServerBulkInsert {
        +SqlBulkCopyOptions Options
        +Insert()
        +InsertAsync()
    }

    class MsSqlServerBulkInsert {
        +SqlBulkCopyOptions Options
        +Insert()
        +InsertAsync()
    }

    class MySqlBulkInsert {
        +String SecureFilePriv
        +String DateTimeFormat
        +List~string~ Expressions
        +Insert()
        +InsertAsync()
        -ToCSV() string
    }

    class MySqlConnectorBulkInsert {
        +String SecureFilePriv
        +String DateTimeFormat
        +Insert()
        +InsertAsync()
    }

    class PostgreSqlBulkInsert {
        +Insert()
        +InsertAsync()
        -InsertImpl()
    }

    IBulkInsert <|.. AbstractBulkInsert
    AbstractBulkInsert <|-- SqlServerBulkInsert
    AbstractBulkInsert <|-- MsSqlServerBulkInsert
    AbstractBulkInsert <|-- MySqlBulkInsert
    AbstractBulkInsert <|-- MySqlConnectorBulkInsert
    AbstractBulkInsert <|-- PostgreSqlBulkInsert

    style IBulkInsert fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style AbstractBulkInsert fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style SqlServerBulkInsert fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style MsSqlServerBulkInsert fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style MySqlBulkInsert fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style MySqlConnectorBulkInsert fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style PostgreSqlBulkInsert fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
```

<!-- Sources: src/SmartSql.Bulk/IBulkInsert.cs:8, src/SmartSql.Bulk/AbstractBulkInsert.cs:10, src/SmartSql.Bulk.SqlServer/BulkInsert.cs:17, src/SmartSql.Bulk.MySql/BulkInsert.cs:19, src/SmartSql.Bulk.PostgreSql/BulkInsert.cs:10 -->

## How Each Database Provider Works

```mermaid
sequenceDiagram
autonumber
    participant App as Application
    participant Bulk as IBulkInsert
    participant Session as IDbSession
    participant DB as Database

    App->>Bulk: Table = dataTable (or Insert(list))
    Bulk->>Session: Open()
    Session->>DB: Open connection

    alt SQL Server
        Bulk->>DB: SqlBulkCopy.WriteToServer(Table)
    else MySQL / MySqlConnector
        Bulk->>Bulk: Write Table to CSV file
        Bulk->>DB: MySqlBulkLoader.Load(fileName)
        Bulk->>Bulk: Delete temp CSV file
    else PostgreSQL
        Bulk->>DB: BeginBinaryImport(COPY ... FROM STDIN)
        loop Each DataRow
            Bulk->>DB: writer.StartRow() + writer.Write(cell)
        end
        Bulk->>DB: writer.Complete()
    end

    Bulk-->>App: Done
```

<!-- Sources: src/SmartSql.Bulk.SqlServer/BulkInsert.cs:28, src/SmartSql.Bulk.MySql/BulkInsert.cs:27, src/SmartSql.Bulk.PostgreSql/BulkInsert.cs:19 -->

## Data Flow

```mermaid
flowchart TD
    subgraph Input["Data Input Options"]
        style Input fill:#161b22,stroke:#30363d,color:#e6edf3
        Entities["IEnumerable~TEntity~"] --> ToDT["BulkExtensions.ToDataTable()"]
        ManualDT["DataTable (manual)"]
    end

    subgraph Process["Bulk Insert Process"]
        style Process fill:#161b22,stroke:#30363d,color:#e6edf3
        ToDT --> DT["DataTable"]
        ManualDT --> DT
        DT --> SetTable["IBulkInsert.Table = dataTable"]
        SetTable --> Insert["Insert() or InsertAsync()"]
        Insert --> Open["DbSession.Open()"]
        Open --> Native["Native bulk copy API"]
    end

    style Entities fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style ToDT fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style ManualDT fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style DT fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style SetTable fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style Insert fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style Open fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style Native fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
```

<!-- Sources: src/SmartSql.Bulk/BulkExtensions.cs:17, src/SmartSql.Bulk/BulkExtensions.cs:58 -->

## Usage

### Convert Entities to DataTable

The `BulkExtensions.ToDataTable<T>()` extension method converts an `IEnumerable<T>` to a `DataTable`, using SmartSql's entity metadata cache for column definitions:

```csharp
var entities = new List<User> { /* ... */ };
DataTable dataTable = entities.ToDataTable();
```

### Bulk Insert Directly from Entity List

The `Insert<T>()` and `InsertAsync<T>()` extension methods combine conversion and insertion:

```csharp
// SQL Server
var bulkInsert = new SmartSql.Bulk.SqlServer.BulkInsert(dbSession);
await bulkInsert.InsertAsync(userList);

// PostgreSQL
var bulkInsert = new SmartSql.Bulk.PostgreSql.BulkInsert(dbSession);
await bulkInsert.InsertAsync(orderList);
```

### Bulk Insert with DataTable

```csharp
var bulkInsert = new SmartSql.Bulk.MySql.BulkInsert(dbSession)
{
    SecureFilePriv = "/tmp",
    DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
};
bulkInsert.Table = dataTable;
bulkInsert.Insert();
```

## Provider-Specific Options

### MySQL / MySqlConnector

| Property | Type | Default | Description |
|---|---|---|---|
| `SecureFilePriv` | `string` | AppDomain base directory | Directory for temp CSV files |
| `DateTimeFormat` | `string` | `"yyyy-MM-dd HH:mm:ss"` | DateTime format in CSV |
| `Expressions` | `List<string>` | empty | MySqlBulkLoader expressions |
| `_fieldTerminator` | `string` | `","` | CSV field delimiter |
| `_fieldQuotationCharacter` | `char` | `"` | CSV quote character |
| `_escapeCharacter` | `char` | `"` | CSV escape character |
| `_lineTerminator` | `string` | `"\r\n"` | CSV line terminator |

### SQL Server / MsSqlServer

| Property | Type | Default | Description |
|---|---|---|---|
| `Options` | `SqlBulkCopyOptions` | `Default` | Bulk copy behavior flags |

### PostgreSQL

PostgreSQL bulk insert uses `NpgsqlConnection.BeginBinaryImport()` with the `COPY ... FROM STDIN (FORMAT BINARY)` command. For JSONB columns, set the `DataTypeName` extended property on the `DataColumn`:

```csharp
DataColumn col = dataTable.Columns["Metadata"];
col.ExtendedProperties.Add("DataTypeName", "JSONB");
```

## Package Mapping

| NuGet Package | Underlying Driver | Mechanism |
|---|---|---|
| `SmartSql.Bulk.SqlServer` | `System.Data.SqlClient` | `SqlBulkCopy` |
| `SmartSql.Bulk.MsSqlServer` | `Microsoft.Data.SqlClient` | `SqlBulkCopy` |
| `SmartSql.Bulk.MySql` | `MySql.Data.MySqlClient` | `MySqlBulkLoader` via CSV |
| `SmartSql.Bulk.MySqlConnector` | `MySqlConnector` | `MySqlBulkLoader` via CSV |
| `SmartSql.Bulk.PostgreSql` | `Npgsql` | `BeginBinaryImport` (COPY BINARY) |

::: info
`SmartSql.Bulk.SqlServer` and `SmartSql.Bulk.MsSqlServer` use the same source file with conditional compilation (`#if MicrosoftSqlClient`). Choose the package matching your SqlClient dependency.
:::

## Cross-References

- **[Type Handlers](./type-handlers.md)** -- Custom type handlers affect how entity properties are converted to `DataTable` values via `BulkExtensions.ToDataTable<T>()`.
- **[DI Integration](./di-extension.md)** -- Register `IBulkInsert` implementations in the DI container.

## References

- [IBulkInsert.cs](https://github.com/dotnetcore/SmartSql/blob/master/src/SmartSql.Bulk/IBulkInsert.cs) -- Interface definition
- [AbstractBulkInsert.cs](https://github.com/dotnetcore/SmartSql/blob/master/src/SmartSql.Bulk/AbstractBulkInsert.cs) -- Base class with `IDbSession`
- [BulkExtensions.cs](https://github.com/dotnetcore/SmartSql/blob/master/src/SmartSql.Bulk/BulkExtensions.cs) -- `ToDataTable<T>()` and `Insert<T>()` extensions
- [SqlServer/BulkInsert.cs](https://github.com/dotnetcore/SmartSql/blob/master/src/SmartSql.Bulk.SqlServer/BulkInsert.cs) -- SQL Server implementation
- [MySql/BulkInsert.cs](https://github.com/dotnetcore/SmartSql/blob/master/src/SmartSql.Bulk.MySql/BulkInsert.cs) -- MySQL implementation
- [PostgreSql/BulkInsert.cs](https://github.com/dotnetcore/SmartSql/blob/master/src/SmartSql.Bulk.PostgreSql/BulkInsert.cs) -- PostgreSQL implementation
