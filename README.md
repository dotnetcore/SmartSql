# 简介

>[SmartSql-Starter](https://github.com/Ahoo-Wang/SmartSql-Starter)

## 0. Why

- 拥抱 跨平台 DotNet Core，是时候了。
- 把你的双手从数据访问层解放出来，少加点班吧。

---

## 1. So SmartSql

- TargetFrameworks: .NETFramework 4.6 & .NETStandard 2.0
- SmartSql = MyBatis + Cache(Memory | Redis) + ZooKeeper + R/W Splitting + ......

---

## 2. 主要特性 (√ 为已完成，未打 √ 为计划特性)

- 1 ORM
  - 1.1 Sync
  - 1.2 Async
- 2 XmlConfig & XmlStatement -> Sql
  - 2.1 SmartSqlMapConfig & SmartSqlMap √  (是的，你猜对了，和MyBatis一样，通过XML配置分离SQL。)
  - 2.2 Config Hot Update ->ConfigWatcher & Reload (配置文件热更新：当你需要修改Sql的时候，直接修改SqlMap配置文件，保存即可。)
- 3 读写分离 √
  - 3.1 读写分离 √
  - 3.2 多读库 权重筛选 √ （配置多读库，根据读库权重选举读库）
- 4 日志 √
  - 4.1 基于 Microsoft.Extensions.Logging.Abstractions  (当你需要跟踪调试的时候一切都是那么一目了然)
- 5 Dynamic Repository
  - 5.1 SmartSql.DyRepository  √
- 6 查询缓存  √
  - 6.1 SmartSql.Cache.Memory  √
    - 6.1.1 Fifo  √
    - 6.1.2 Lru  √
  - 6.2 SmartSql.Cache.Redis  √
  - 6.3 缓存事务一致性
- 7 分布式配置插件
  - 7.1 IConfigLoader √ (配置文件加载器)
  - 7.2 LocalFileConfigLoader  √ √ (本地文件配置加载器)
    - 7.2.1 Load SmartSqlMapSource Xml  √
    - 7.3.1 Load SmartSqlMapSource Directory √
  - 7.3 SmartSql.ZooKeeperConfig √ (ZooKeeper 分布式配置文件加载器)

---

## 3. 性能

``` ini

BenchmarkDotNet=v0.10.14, OS=Windows 10.0.16299.431 (1709/FallCreatorsUpdate/Redstone3)
Intel Core i7-4710MQ CPU 2.50GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
Frequency=2435768 Hz, Resolution=410.5481 ns, Timer=TSC
.NET Core SDK=2.1.201
  [Host]     : .NET Core 2.0.7 (CoreCLR 4.6.26328.01, CoreFX 4.6.26403.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.7 (CoreCLR 4.6.26328.01, CoreFX 4.6.26403.03), 64bit RyuJIT

```

|            ORM |                     Type |                  Method |     Mean |     Error |    StdDev | Rank |      Gen 0 |     Gen 1 |     Gen 2 | Allocated |
|--------------- |------------------------- |------------------------ |---------:|----------:|----------:|-----:|-----------:|----------:|----------:|----------:|
|       SmartSql |       SmartSqlBenchmarks |                   Query | 101.6 ms | 0.2226 ms | 0.1738 ms |    1 |  2437.5000 | 1062.5000 |  375.0000 |  13.37 MB |
|         Native |         NativeBenchmarks | Query_IsDBNull_GetValue | 101.7 ms | 0.4101 ms | 0.3635 ms |    1 |  2437.5000 | 1062.5000 |  375.0000 |  13.37 MB |
|         Dapper |         DapperBenchmarks |                   Query | 104.4 ms | 1.3195 ms | 1.2342 ms |    2 |  3375.0000 | 1375.0000 |  625.0000 |  17.64 MB |
| SmartSqlDapper | SmartSqlDapperBenchmarks |                   Query | 105.7 ms | 1.1697 ms | 1.0941 ms |    3 |  3750.0000 | 1437.5000 |  625.0000 |  19.47 MB |
|         Native |         NativeBenchmarks |   Query_GetValue_DbNull | 107.4 ms | 1.0710 ms | 1.0018 ms |    4 |  3062.5000 | 1187.5000 |  500.0000 |  16.42 MB |
|       SqlSugar |       SqlSugarBenchmarks |                   Query | 108.9 ms | 0.4048 ms | 0.3787 ms |    5 |  2375.0000 | 1000.0000 |  312.5000 |  13.09 MB |
|             EF |             EFBenchmarks |                SqlQuery | 110.9 ms | 0.6922 ms | 0.6475 ms |    6 | 11062.5000 |         - |         - |  34.13 MB |
|          Chloe |          ChloeBenchmarks |                   Query | 14.5 ms  | 2.2600 ms | 5.3711 ms |    7 |  2375.0000 | 1000.0000 |  312.5000 |  13.07 MB |
|             EF |             EFBenchmarks |        Query_NoTracking | 126.4 ms | 1.3197 ms | 1.2344 ms |    8 |  5937.5000 | 2250.0000 | 1062.5000 |  30.16 MB |
|             EF |             EFBenchmarks |     SqlQuery_NoTracking | 148.6 ms | 0.8290 ms | 0.7755 ms |    9 |  7437.5000 | 2937.5000 | 1250.0000 |  37.79 MB |

---

## 4. 安装 (NuGet)

``` csharp
Install-Package SmartSql
```

## 5. 常规代码

### 查询

``` csharp
            ISmartSqlMapper SqlMapper = MapperContainer.Instance.GetSqlMapper();
            SqlMapper.Query<T_Test>(new RequestContext
            {
                Scope = "T_Test",
                SqlId = "GetList",
                Request = new { Ids = new long[] { 1, 2, 3, 4 } }
            });
```

### 事务

``` csharp
            try
            {
                ISmartSqlMapper SqlMapper = MapperContainer.Instance.GetSqlMapper();
                SqlMapper.BeginTransaction();
                //BizCode
                SqlMapper.CommitTransaction();
            }
            catch (Exception ex)
            {
                SqlMapper.RollbackTransaction();
                throw ex;
            }
```

## 6. 最佳实践

### 6.1 安装 SmartSql.DIExtension

``` chsarp
Install-Package SmartSql.DIExtension
```

### 6.2 注入依赖

``` csharp
 services.AddSmartSql();
 services.AddRepositoryFactory();
 services.AddRepositoryFromAssembly((options) =>
 {
    options.AssemblyString = "SmartSql.Starter.Repository";
 });
```

### 6.3 定义仓储接口

``` csharp
    /// <summary>
    /// 属性可选： [SqlMap(Scope = "User")] ,不设置 则默认 Scope 模板：I{Scope}Repository
    /// 可传入自定义模板
    /// RepositoryBuilder builder=new RepositoryBuilder("I{Scope}DAL");
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// 属性可选 [Statement(Execute = ExecuteBehavior.Auto,Id = "Query")]
        /// 默认 Execute：Auto ，自动判断 执行类型
        /// 默认 Id : 方法名
        /// </summary>
        /// <param name="reqParams"></param>
        /// <returns></returns>
        IEnumerable<User> Query(object reqParams);
        long GetRecord(object reqParams);
        User Get(object reqParams);
        long Insert(User entity);
        int Update(User entity);
        int Delete(User enttiy);
    }
```

### 6.4 尽情享用

``` csharp
    public class UserService
    {
        private readonly ISmartSqlMapper _smartSqlMapper;
        private readonly IUserRepository _userRepository;

        public UserService(
             ISmartSqlMapper smartSqlMapper
            , IUserRepository userRepository)
        {
            _smartSqlMapper = smartSqlMapper;
            _userRepository = userRepository;
        }

        public long Add(AddRequest request)
        {
            int existsNum = _userRepository.Exists(new { request.UserName });
            if (existsNum > 0)
            {
                throw new ArgumentException($"{nameof(request.UserName)} has already existed!");
            }
            return _userRepository.Add(new Entitiy.User
            {
                UserName = request.UserName,
                Password = request.Password,
                Status = Entitiy.UserStatus.Ok,
                CreationTime = DateTime.Now,
            });
        }

        public void UseTransaction()
        {
            try
            {
                _smartSqlMapper.BeginTransaction();
                //Biz();
                _smartSqlMapper.CommitTransaction();
            }
            catch (Exception ex)
            {
                _smartSqlMapper.RollbackTransaction();
                throw ex;
            }
        }
    }
```

## 7. 文档地址

- [在线阅读地址](https://doc.smartsql.net/)
- [PDF](https://www.gitbook.com/download/pdf/book/ahoo-wang/smartsql-doc-cn)
- [Mobi](https://www.gitbook.com/download/mobi/book/ahoo-wang/smartsql-doc-cn)
- [ePub](https://www.gitbook.com/download/epub/book/ahoo-wang/smartsql-doc-cn)

## 8. 技术交流

点击链接加入QQ群【SmartSql 官方交流群】：[604762592](https://jq.qq.com/?_wv=1027&k=5Sy8Ahw)