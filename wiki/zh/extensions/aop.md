---
title: AOP 事务
description: 使用 AspectCore 拦截器属性实现声明式事务管理
---

# AOP 事务

`SmartSql.AOP` 包通过 AspectCore 拦截器属性提供声明式事务和会话管理。你无需编写显式的 `using` / `TransactionWrap` 代码块，只需用 `[Transaction]` 或 `[DbSession]` 属性装饰你的方法，让 AOP 框架处理会话打开、事务提交和异常回滚。

## 一览表

| 特性 | 描述 |
|---------|-------------|
| 包名 | `SmartSql.AOP` |
| 框架 | AspectCore DynamicProxy |
| `[Transaction]` | 打开 DB 会话，将方法包装在事务中 |
| `[DbSession]` | 打开 DB 会话（无事务） |
| 嵌套支持 | 检测已有事务，避免重复包装 |
| 隔离级别 | 通过 `Level` 属性可配置 |
| 多实例 | `Alias` 属性选择使用哪个 SmartSql 实例 |

## 事务生命周期

`[Transaction]` 属性管理数据库事务的完整生命周期：

```mermaid
sequenceDiagram
autonumber
    participant Caller as Service Method
    participant Attr as TransactionAttribute
    participant Store as IDbSessionStore
    participant Session as IDbSession
    participant DB as Database

    Caller->>Attr: Invoke(context, next)
    Attr->>Store: GetSessionStore(Alias)
    Store-->>Attr: sessionStore

    alt sessionStore is null
        Attr->>Attr: throw SmartSqlException
    end

    Attr->>Store: LocalSession?.Transaction
    alt Already in transaction
        Attr->>Caller: next.Invoke(context) -- pass through
    else No active transaction
        Attr->>Store: Open()
        Store-->>Session: session (LocalSession)
        Attr->>Session: TransactionWrapAsync(Level, handler)
        Session->>DB: BEGIN TRANSACTION
        Session->>Caller: next.Invoke(context)
        alt Success
            Session->>DB: COMMIT
        else Exception
            Session->>DB: ROLLBACK
            Session->>Caller: Propagate exception
        end
        Attr->>Store: Dispose (close session)
    end
```

<!-- Sources: src/SmartSql.AOP/TransactionAttribute.cs:13, src/SmartSql.AOP/DbSessionAttribute.cs:10 -->

## 事务嵌套

SmartSql 的 AOP 事务系统支持嵌套。当标注了 `[Transaction]` 的方法调用另一个 `[Transaction]` 方法时，内部调用会检测到已有事务并直接通过，不会创建嵌套事务：

```mermaid
flowchart TD
    subgraph Nesting["Transaction Nesting Behavior"]
        style Nesting fill:#161b22,stroke:#30363d,color:#e6edf3
        M1["OuterMethod() [Transaction]"] --> Check1{"sessionStore.LocalSession<br>has Transaction?"}
        Check1 -->|No| Open["Open session + BEGIN TX"]
        Open --> Call["Call InnerMethod()"]
        Call --> Check2{"sessionStore.LocalSession<br>has Transaction?"}
        Check2 -->|Yes| Pass["Pass through (no nested TX)"]
        Pass --> Return["Inner returns"]
        Return --> Commit["COMMIT outer TX"]
        Check1 -->|Yes| Error["Error: should not happen<br>at outer level"]
    end

    style M1 fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style Check1 fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style Open fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style Call fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style Check2 fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style Pass fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style Return fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style Commit fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style Error fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
```

<!-- Sources: src/SmartSql.AOP/TransactionAttribute.cs:24, src/SmartSql.AOP/TransactionAttribute.cs:28 -->

## 属性

### `[Transaction]`

拦截方法调用并将其包装在数据库事务中：

```csharp
public class UserService
{
    private readonly IUserRepository _userRepository;

    [Transaction(Alias = "SmartSql", Level = IsolationLevel.ReadCommitted)]
    public void TransferFunds(long fromId, long toId, decimal amount)
    {
        var from = _userRepository.GetById(fromId);
        var to = _userRepository.GetById(toId);
        from.Balance -= amount;
        to.Balance += amount;
        _userRepository.Update(from);
        _userRepository.Update(to);
    }
}
```

| 属性 | 类型 | 默认值 | 描述 |
|---|---|---|---|
| `Alias` | `string` | `"SmartSql"` | 使用哪个 SmartSql 实例 |
| `Level` | `IsolationLevel` | `Unspecified` | 事务隔离级别 |

### `[DbSession]`

打开数据库会话但不开启事务。适用于受益于连接复用的只读操作：

```csharp
[DbSession(Alias = "SmartSql")]
public async Task<IList<User>> GetAllUsers()
{
    return await _userRepository.QueryAsync();
}
```

| 属性 | 类型 | 默认值 | 描述 |
|---|---|---|---|
| `Alias` | `string` | `"SmartSql"` | 使用哪个 SmartSql 实例 |

## 与 AspectCore 集成

两个属性都继承自 `AspectCore.DynamicProxy.AbstractInterceptorAttribute`。要在应用中启用它们，必须将 AspectCore 配置为 DI 提供程序：

```csharp
public IServiceProvider ConfigureServices(IServiceCollection services)
{
    services.AddSmartSql("SmartSql")
        .AddRepositoryFromAssembly(o =>
        {
            o.AssemblyString = "MyApp";
        });

    // Register services that use [Transaction]
    services.AddSingleton<UserService>();

    // Use AspectCore's proxy provider
    return services.BuildAspectInjectorProvider();
}
```

::: warning
如果没有 `BuildAspectInjectorProvider()`，`[Transaction]` 和 `[DbSession]` 属性将无法拦截方法调用。你必须使用 AspectCore 的服务提供程序。
:::

## DyRepository 与 AOP 事务对比

SmartSql 提供了两种声明事务的方式：

```mermaid
flowchart LR
    subgraph Approaches["Transaction Approaches"]
        style Approaches fill:#161b22,stroke:#30363d,color:#e6edf3
        A["[UseTransaction]"] --> B["For repository interfaces<br>DyRepository layer"]
        C["[Transaction]"] --> D["For service classes<br>Service layer (AOP)"]
    end

    style A fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style B fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style C fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
    style D fill:#2d333b,stroke:#6d5dfc,color:#e6edf3
```

| 特性 | `[UseTransaction]` | `[Transaction]` |
|---|---|---|
| 应用于 | 仓储接口方法 | 任意服务方法 |
| 需要 AspectCore | 否 | 是 |
| 嵌套感知 | 通过 DyRepository emit 逻辑 | 通过 `sessionStore.LocalSession` 检查 |
| 推荐层 | 数据访问 | 业务/服务层 |

参见 [动态仓储](./dy-repository.md) 页面了解 `[UseTransaction]` 的详情。

## API 参考

### TransactionAttribute

| 成员 | 类型 | 描述 |
|---|---|---|
| `Alias` | `string` | SmartSql 实例别名 |
| `Level` | `IsolationLevel` | 事务隔离级别 |
| `Invoke(AspectContext, AspectDelegate)` | `Task` | 拦截器入口点 |

### DbSessionAttribute

| 成员 | 类型 | 描述 |
|---|---|---|
| `Alias` | `string` | SmartSql 实例别名 |
| `Invoke(AspectContext, AspectDelegate)` | `Task` | 拦截器入口点 |

## 交叉参考

- **[动态仓储](./dy-repository.md)** -- 仓储方法的 `[UseTransaction]` 注解。
- **[DI 集成](./di-extension.md)** -- 如何注册使用 AOP 属性的服务。
- **[InvokeSync](./invoke-sync.md)** -- 事务事件可触发数据同步。

## 参考资料

- [TransactionAttribute.cs](https://github.com/dotnetcore/SmartSql/blob/master/src/SmartSql.AOP/TransactionAttribute.cs)
- [DbSessionAttribute.cs](https://github.com/dotnetcore/SmartSql/blob/master/src/SmartSql.AOP/DbSessionAttribute.cs)
- [Startup.cs](https://github.com/dotnetcore/SmartSql/blob/master/sample/SmartSql.Sample.AspNetCore/Startup.cs) -- 使用 `BuildAspectInjectorProvider()` 的示例
