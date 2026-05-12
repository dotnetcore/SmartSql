---
title: 更新日志
description: SmartSql 版本历史、关键里程碑和发布说明。当前版本 4.1.68。
---

# 更新日志

SmartSql 在 [github.com/dotnetcore/SmartSql](https://github.com/dotnetcore/SmartSql) 上积极维护。当前版本为 **4.1.68**，在 `build/version.props` 中管理。

## 版本 4.1.68

**安全更新**：将 `Npgsql` 依赖更新至 4.1.13 以解决 [CVE 安全公告](https://github.com/dotnetcore/SmartSql/commit/ebb9727)。

## 关键里程碑

| 里程碑 | 描述 |
|--------|------|
| **初始发布** | 核心 ORM，支持 XML 管理的 SQL、`ISqlMapper`、中间件管道架构 |
| **读写分离** | `DataSourceFilter` 支持跨读副本的加权负载均衡（[DataSourceFilter.cs](https://github.com/dotnetcore/SmartSql/blob/master/src/SmartSql/DataSource/DataSourceFilter.cs)） |
| **缓存** | 内置 LRU 和 FIFO 内存缓存提供程序，通过 `SmartSql.Cache.Redis` 支持 Redis 缓存（[Cache.cs](https://github.com/dotnetcore/SmartSql/blob/master/src/SmartSql/Configuration/Cache.cs)） |
| **动态仓储** | 基于 IL emit 的接口到实现的代理生成（[DyRepository](https://github.com/dotnetcore/SmartSql/tree/master/src/SmartSql.DyRepository)） |
| **CUD 扩展** | 基于约定的 Insert/Update/Delete/GetById，无需 XML（[CUDSqlGenerator.cs](https://github.com/dotnetcore/SmartSql/blob/master/src/SmartSql/CUD/CUDSqlGenerator.cs)） |
| **批量插入** | 原生批量复制，支持 SqlServer、MsSqlServer、MySql、MySqlConnector 和 PostgreSQL |
| **诊断** | `DiagnosticSource` 事件用于命令执行、会话生命周期和错误的 APM 工具集成 |
| **AOP 事务** | `[Transaction]` 属性用于声明式事务管理（`SmartSql.AOP`） |
| **缓存同步** | 通过 `SmartSql.Cache.Sync` 结合 Kafka/RabbitMQ 实现跨实例缓存失效 |
| **数据同步** | `SmartSql.InvokeSync` 结合 Kafka 和 RabbitMQ 发布者实现事件驱动同步 |
| **属性变更跟踪** | `EnablePropertyChangedTrack` 用于仅修改变更字段的部分 UPDATE 语句 |
| **自动转换器** | `IAutoConverter` 系统用于自动参数转换 |
| **多结果集** | `MultipleResultMap` 用于单次往返中返回数据 + 计数的分页查询 |
| **ID 生成** | 内置 SnowflakeId 生成器，支持 `IdGenerator` 标签 |
| **嵌套属性映射** | `ResultMap` 中的点号属性路径（例如 `Prop1.Prop2.Prop3`） |
| **.NET 6 支持** | 更新为同时以 `netstandard2.0` 和 `net6.0` 为目标用于测试项目 |

## 发布历史

### 近期发布

- **4.1.68** -- 安全性：将 Npgsql 更新至 4.1.13
- **4.1.67** -- 递增补丁版本
- **4.1.66** -- `IPropertyHolder` Property 设置为只读；移除无用注释
- **4.1.65** -- 修正文件字符编码从 GBK 到 UTF-8

### 核心库稳定性

核心 `SmartSql` 包（`src/SmartSql/`）以 `netstandard2.0` 为目标并保持向后兼容。扩展包独立版本化。

## 包版本

版本在 [build/version.props](https://github.com/dotnetcore/SmartSql/blob/master/build/version.props) 中集中管理：

```xml
<Project>
  <PropertyGroup>
    <Version>4.1.68</Version>
  </PropertyGroup>
</Project>
```

## 相关页面

- [介绍](./index.md) -- 架构概览
- [快速上手](./quick-start.md) -- 入门指南
- [配置](./configuration.md) -- 完整配置参考
- [XML SQL 映射](./xml-sql-maps.md) -- 动态 SQL 标签参考

## 参考资料

- [SmartSql GitHub 仓库](https://github.com/dotnetcore/SmartSql)
- [build/version.props](https://github.com/dotnetcore/SmartSql/blob/master/build/version.props) -- 版本定义
- [SmartSql.sln](https://github.com/dotnetcore/SmartSql/blob/master/SmartSql.sln) -- 解决方案文件
