# 集成测试重构设计方案

## 背景

当前 `SmartSql.Test.Integration` 项目存在以下问题：
1. **大量测试与单元测试重复**：Tags、Cache、Deserializer 等目录下的大量测试仅调用 `BuildSql` 或解析配置，从不真正访问数据库，与 `SmartSql.Test.Unit` 高度重叠。
2. **仅支持单一数据库**：所有测试固定使用 MySQL 8.0 + Redis 7，无法验证其他数据库（PostgreSQL、SQL Server、SQLite）的兼容性。

## 目标

1. 移除集成测试中与单元测试重复的内容，保持测试职责清晰。
2. 支持多数据库（MySQL 8、PostgreSQL 16、SQL Server 2022、SQLite in-memory）的集成测试。
3. 通过 xUnit Collection + 抽象基类模式实现代码复用。

---

## 项目结构

```
src/SmartSql.Test.Integration/
├── DB/
│   ├── init-mysql-db.sql          # 已有
│   ├── init-postgresql-db.sql     # 新增
│   ├── init-sqlserver-db.sql      # 新增
│   └── init-sqlite-db.sql         # 新增
├── Maps/
│   ├── AllPrimitive.xml           # 改造：用 <Env> 处理 DB 差异
│   ├── T_Entity.xml               # 改造
│   ├── User.xml                   # 改造
│   ├── UserExtendedInfo.xml
│   ├── NestTest.xml
│   ├── TagTest.xml
│   ├── UseIdGenEntity.xml
│   ├── AssignAutoConverter.xml
│   ├── CustomizeTypeHandlerTest.xml
│   ├── DefaultAutoConverter.xml
│   ├── DisabledAutoConverter.xml
│   ├── FifoCache.xml
│   ├── LruCache.xml
│   ├── RedisCache.xml             # 保持不变（仅 MySQL）
│   └── ...
├── Fixtures/
│   ├── IDbTestFixture.cs          # 接口：暴露 ISqlMapper, SmartSqlBuilder, DbProvider
│   ├── MySqlFixture.cs           # Testcontainers MySQL 8.0
│   ├── PostgreSqlFixture.cs      # Testcontainers PostgreSQL 16
│   ├── SqlServerFixture.cs       # Testcontainers SQL Server 2022
│   ├── SqliteFixture.cs          # in-memory，无需 Docker
│   └── RedisFixture.cs           # 独立 Redis fixture（仅缓存测试用）
├── Base/
│   ├── CUDTestBase.cs             # Insert/Update/Delete 测试
│   ├── SqlMapperTestBase.cs      # Query/QuerySingle 测试
│   ├── DbSessionTestBase.cs      # 事务、SP、RealSql 测试
│   ├── DyRepositoryTestBase.cs   # 动态 Repository 测试
│   ├── CacheTestBase.cs          # Redis 缓存测试（仅 MySQL）
│   ├── TypeHandlerTestBase.cs   # 自定义类型处理器测试
│   ├── BulkTestBase.cs          # Bulk Insert 测试
│   └── DeserializerFactoryTestBase.cs  # 自定义 Deserializer 测试
├── MySql/
│   ├── MySqlCUDTests.cs         # : CUDTestBase
│   ├── MySqlSqlMapperTests.cs
│   ├── MySqlDbSessionTests.cs
│   ├── MySqlDyRepositoryTests.cs
│   ├── MySqlCacheTests.cs
│   ├── MySqlTypeHandlerTests.cs
│   ├── MySqlBulkTests.cs
│   └── MySqlDeserializerFactoryTests.cs
├── PostgreSql/
│   ├── PgCUDTests.cs
│   ├── PgSqlMapperTests.cs
│   ├── PgDbSessionTests.cs
│   ├── PgDyRepositoryTests.cs
│   ├── PgBulkTests.cs
│   └── PgDeserializerFactoryTests.cs
├── SqlServer/
│   ├── SqlServerCUDTests.cs
│   ├── SqlServerSqlMapperTests.cs
│   ├── SqlServerDbSessionTests.cs
│   ├── SqlServerDyRepositoryTests.cs
│   ├── SqlServerBulkTests.cs
│   └── SqlServerDeserializerFactoryTests.cs
├── Sqlite/
│   ├── SqliteCUDTests.cs
│   ├── SqliteSqlMapperTests.cs
│   ├── SqliteDbSessionTests.cs
│   ├── SqliteDyRepositoryTests.cs
│   └── SqliteDeserializerFactoryTests.cs
├── DI/
│   └── DITests.cs                # 保持不变，无 DB 依赖
├── IdGenerator/
│   ├── SnowflakeIdTests.cs       # 保持不变，内存生成
│   └── CustomSnowflakeIdTests.cs # 保持不变
├── EnvironmentFactAttribute.cs   # 保留
├── TestPrepareStatementFilter.cs  # 视需要保留
└── SmartSql.Test.Integration.csproj
```

---

## Fixture 抽象

### IDbTestFixture 接口

```csharp
public interface IDbTestFixture : IAsyncLifetime
{
    ISqlMapper SqlMapper { get; }
    SmartSqlBuilder SmartSqlBuilder { get; }
    string DbProvider { get; }
}
```

### 各 Fixture 职责

| Fixture | 容器 | 初始化逻辑 |
|---------|------|-----------|
| `MySqlFixture` | `MySqlContainer` (8.0) | 启动容器 → 执行 `init-mysql-db.sql` → 创建 SP → 构建 `SmartSqlBuilder` |
| `PostgreSqlFixture` | `PostgreSqlContainer` (16) | 启动容器 → 执行 `init-postgresql-db.sql` → 构建 `SmartSqlBuilder` |
| `SqlServerFixture` | `SqlServerContainer` (2022) | 启动容器 → 执行 `init-sqlserver-db.sql` → 构建 `SmartSqlBuilder` |
| `SqliteFixture` | 无 | `UseDataSource(DbProvider.SQLITE, "Data Source=:memory:")` → 执行 init → 构建 builder |
| `RedisFixture` | `RedisContainer` (7) | 仅用于 `CacheTests`，独立 xUnit Collection |

### xUnit Collection 注册

```csharp
[CollectionDefinition("MySql")]
public class MySqlCollection : ICollectionFixture<MySqlFixture>;

[CollectionDefinition("PostgreSql")]
public class PgCollection : ICollectionFixture<PostgreSqlFixture>;

[CollectionDefinition("SqlServer")]
public class SqlServerCollection : ICollectionFixture<SqlServerFixture>;

[CollectionDefinition("Sqlite")]
public class SqliteCollection : ICollectionFixture<SqliteFixture>;

[CollectionDefinition("Redis")]
public class RedisCollection : ICollectionFixture<RedisFixture>;
```

---

## 保留的测试类别

| 测试基类 | 来源 | 测试内容 |
|---------|------|---------|
| `CUDTestBase` | `CUDTests.cs` | Insert/GetById、Insert 返回 ID、Update、DyUpdate、DeleteById、DeleteMany、PropertyChangedTrack |
| `SqlMapperTestBase` | `SqlMapperTests.cs` | Query/QueryAsync、QuerySingle、QueryDynamic、QueryDictionary、PropertyChangedTrack |
| `DbSessionTestBase` | `DbSessionTests.cs` | RealSql 插入、Statement 插入（含事务）、IdGen 插入、Async Insert、SP 调用（`[EnvironmentFact]` 标记，Sqlite 可 skip） |
| `DyRepositoryTestBase` | `DyRepository/*` | AllPrimitiveRepository CRUD、ColumnAnnotationRepository、UserRepository（SP）、UsedCacheRepository |
| `CacheTestBase` | `RedisCacheProviderTests.cs` | Redis 缓存命中验证（仅 MySQL，使用 RedisFixture） |
| `TypeHandlerTestBase` | `CustomizeTypeHandlerTests.cs` | AnsiString/AnsiStringFixedLength 类型处理 |
| `BulkTestBase` | `Bulk/*` | 各 DB Provider 的 bulk insert 实现（SQLite 无 bulk） |
| `DeserializerFactoryTestBase` | `DeserializerFactoryTests.cs` | 自定义 Deserializer 注册 |
| `DITests` | `DITests.cs` | DI 容器解析（不变，无 DB 依赖） |
| `SnowflakeIdTests` | `IdGenerator/*` | 雪花 ID 生成（不变，纯内存） |

---

## 需要删除的测试文件

以下测试已在单元测试中覆盖，不涉及真实数据库访问：

- `Tags/*` 全部 15 个文件
- `Cache/FifoCacheProviderTests.cs`、`Cache/LruCacheProviderTests.cs`
- `Deserializer/` 下除 `DeserializerFactoryTests.cs` 外的全部 7 个文件
- `SmartSqlBuilderTests.cs`、`OptionConfigBuilderTests.cs`
- `CUD/CUDConfigBuilderTests.cs`
- `NestTests.cs`（纯 XML 解析）
- `TestPrepareStatementFilter.cs`（视需要保留）

---

## Maps 的 DB 差异处理

使用 SmartSql 的 `<Env>` 标签处理 DB 特定 SQL：

### 自增 ID 获取语法

```xml
<Env Provider="MySql">Select Last_Insert_Id();</Env>
<Env Provider="PostgreSql">RETURNING Id;</Env>
<Env Provider="SqlServer">SELECT SCOPE_IDENTITY();</Env>
<Env Provider="Sqlite">SELECT last_insert_rowid();</Env>
```

### 分页语法

```xml
<Env Provider="MySql">limit ?Taken</Env>
<Env Provider="PostgreSql">LIMIT @Taken</Env>
<Env Provider="SqlServer">TOP @Taken</Env>
<Env Provider="Sqlite">LIMIT @Taken</Env>
```

### 参数前缀

SmartSql 的 `ParameterPrefix` 配置（`Settings.ParameterPrefix`）已支持统一 `$` 前缀自动转换，实际需要 `<Env>` 处理的主要是上述场景。

---

## Init 脚本差异

| 特性 | MySQL | PostgreSQL | SQL Server | SQLite |
|------|-------|------------|------------|--------|
| 自增主键 | `auto_increment` | `GENERATED ALWAYS AS IDENTITY` | `IDENTITY(1,1)` | `AUTOINCREMENT` |
| 布尔类型 | `tinyint(1)` | `BOOLEAN` | `BIT` | `INTEGER` |
| JSON 类型 | `json` + `CHECK(json_valid())` | `jsonb` | `nvarchar(max)` | `TEXT` |
| 时间类型 | `datetime` | `timestamp` | `datetime2` | `TEXT` |
| 存储过程 | 支持 | 支持 | 支持 | 不支持 |

---

## csproj 变更

```xml
<!-- 新增 -->
<PackageReference Include="Testcontainers.SqlServer" Version="4.11.0" />
<PackageReference Include="Testcontainers.PostgreSql" Version="4.11.0" />

<!-- 移除（不再需要） -->
<!-- Jint、Microsoft.CodeAnalysis.CSharp.Scripting（用于 tags/script 动态解析） -->
```

---

## 运行命令

```bash
# 运行所有集成测试（需要 Docker）
dotnet test src/SmartSql.Test.Integration/SmartSql.Test.Integration.csproj

# 只跑特定数据库
dotnet test ... --filter "Collection=MySql"
dotnet test ... --filter "Collection=PostgreSql"
dotnet test ... --filter "Collection=SqlServer"
dotnet test ... --filter "Collection=Sqlite"

# 跳过慢速测试
dotnet test ... --filter "Collection!=SqlServer"
```

---

## 实施步骤

1. 创建 `Fixtures/IDbTestFixture.cs` 接口和各 DB Fixture
2. 创建各 `Base/` 抽象基类
3. 创建 DB 特定子类（MySql/、PostgreSql/、SqlServer/、Sqlite/）
4. 创建各 DB 的 `DB/init-*.sql` 脚本
5. 改造现有 Maps 使用 `<Env>` 处理差异
6. 更新 `SmartSql.Test.Integration.csproj`
7. 删除冗余测试文件
8. 验证测试通过
