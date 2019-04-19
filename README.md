<p align="center">
  <a href="https://www.smartsql.net/" target="_blank">
    <img width="100"src="./SmartSql.png"/>
  </a>
</p>

# SmartSql ([文档地址](https://www.smartsql.net))

# 介绍

SmartSql = MyBatis + Cache(Memory | Redis) + R/W Splitting +Dynamic Repository + Diagnostics ......

## 她是如何工作的？

SmartSql 借鉴了MyBatis的思想，使用XML来管理SQL，并且提供了若干个筛选器标签来消除代码层面的各种if/else的判断分支。

SmartSql将管理你的SQL，并且通过筛选标签来维护本来你在代码层面的各种条件判断，使你的代码更加优美。

同时SmartSql还提供了以下各种特性(包括但不限于)：

- [动态代理仓储](https://www.smartsql.net/dyrepository/)
- 分布式缓存
- 类型处理器
- 自动生成 CUD 代码
- Id生成器
- 性能诊断
- AOP 级别的事物
- 缓存（内存，分布式缓存）
- 读写分离
- 代码生成器(<https://github.com/dotnetcore/SmartCode>)
- 高性能的批量插入

## 为什么选择SmartSql？

DotNet 体系下大都是Linq系的ORM，Linq很好，消除了开发人员对SQL的依赖。但却忽视了一点，SQL本身并不复杂，而且在复杂查询场景当中开发人员很难通过编写Linq来生成良好性能的SQL，相信使用过EF的同学一定有这样的体验：“我想好了Sql怎么写，然后再来写Linq,完了可能还要再查看一下Linq输出的Sql是什么样的“。这是非常糟糕的体验。要想对Sql做绝对的优化，那么开发者必须对Sql有绝对的控制权。另外Sql本身很简单，为何要增加一层翻译器呢？

## 那么为什么不是Dapper，或者DbHelper?

Dapper 确实很好，并且又很好的性能，但是会让给你的代码里边充斥着SQL和各种判断分支，这些将会使代码维护难以阅读和维护。另外 Dapper 只提供了DataReader到Entity的反序列化功能。而SmartSql提供了大量的特性来提升开发者的效率。

## 主要特性

![SmartSql特性](https://raw.githubusercontent.com/Smart-Kit/SmartSql-Docs/master/docs/imgs/SmartSql-features.png)

## 使用 [SmartCode](https://github.com/dotnetcore/SmartCode) 直接体验 [SmartSql](https://github.com/dotnetcore/SmartSql)

![SmartCode](https://raw.githubusercontent.com/Smart-Kit/SmartSql-Docs/master/docs/imgs/SmartCode-Db-1.gif)

## Nuget Packages

| Package | NuGet Stable |  Downloads |
| ------- | -------- | ------- |
| [SmartSql](https://www.nuget.org/packages/SmartSql/) | [![SmartSql](https://img.shields.io/nuget/v/SmartSql.svg)](https://www.nuget.org/packages/SmartSql/)  | [![SmartSql](https://img.shields.io/nuget/dt/SmartSql.svg)](https://www.nuget.org/packages/SmartSql/) |
| [SmartSql.Schema](https://www.nuget.org/packages/SmartSql.Schema/) | [![SmartSql.Schema](https://img.shields.io/nuget/v/SmartSql.Schema.svg)](https://www.nuget.org/packages/SmartSql.Schema/)  | [![SmartSql.Schema](https://img.shields.io/nuget/dt/SmartSql.Schema.svg)](https://www.nuget.org/packages/SmartSql.Schema/) |
| [SmartSql.TypeHandler](https://www.nuget.org/packages/SmartSql.TypeHandler/) | [![SmartSql.TypeHandler](https://img.shields.io/nuget/v/SmartSql.TypeHandler.svg)](https://www.nuget.org/packages/SmartSql.TypeHandler/)  | [![SmartSql.TypeHandler](https://img.shields.io/nuget/dt/SmartSql.TypeHandler.svg)](https://www.nuget.org/packages/SmartSql.TypeHandler/) |
| [SmartSql.DyRepository](https://www.nuget.org/packages/SmartSql.DyRepository/) | [![SmartSql.DyRepository](https://img.shields.io/nuget/v/SmartSql.DyRepository.svg)](https://www.nuget.org/packages/SmartSql.DyRepository/)  | [![SmartSql.DyRepository](https://img.shields.io/nuget/dt/SmartSql.DyRepository.svg)](https://www.nuget.org/packages/SmartSql.DyRepository/) |
| [SmartSql.DIExtension](https://www.nuget.org/packages/SmartSql.DIExtension/) | [![SmartSql.DIExtension](https://img.shields.io/nuget/v/SmartSql.DIExtension.svg)](https://www.nuget.org/packages/SmartSql.DIExtension/)  | [![SmartSql.DIExtension](https://img.shields.io/nuget/dt/SmartSql.DIExtension.svg)](https://www.nuget.org/packages/SmartSql.DIExtension/) |
| [SmartSql.Cache.Redis](https://www.nuget.org/packages/SmartSql.Cache.Redis/) | [![SmartSql.Cache.Redis](https://img.shields.io/nuget/v/SmartSql.Cache.Redis.svg)](https://www.nuget.org/packages/SmartSql.Cache.Redis/)  | [![SmartSql.Cache.Redis](https://img.shields.io/nuget/dt/SmartSql.Cache.Redis.svg)](https://www.nuget.org/packages/SmartSql.Cache.Redis/) |
| [SmartSql.ScriptTag](https://www.nuget.org/packages/SmartSql.ScriptTag/) | [![SmartSql.ScriptTag](https://img.shields.io/nuget/v/SmartSql.ScriptTag.svg)](https://www.nuget.org/packages/SmartSql.ScriptTag/)  | [![SmartSql.ScriptTag](https://img.shields.io/nuget/dt/SmartSql.ScriptTag.svg)](https://www.nuget.org/packages/SmartSql.ScriptTag/) |
| [SmartSql.AOP](https://www.nuget.org/packages/SmartSql.AOP/) | [![SmartSql.AOP](https://img.shields.io/nuget/v/SmartSql.AOP.svg)](https://www.nuget.org/packages/SmartSql.AOP/)  | [![SmartSql.AOP](https://img.shields.io/nuget/dt/SmartSql.AOP.svg)](https://www.nuget.org/packages/SmartSql.AOP/) |
| [SmartSql.Options](https://www.nuget.org/packages/SmartSql.Options/) | [![SmartSql.Options](https://img.shields.io/nuget/v/SmartSql.Options.svg)](https://www.nuget.org/packages/SmartSql.Options/)  | [![SmartSql.Options](https://img.shields.io/nuget/dt/SmartSql.Options.svg)](https://www.nuget.org/packages/SmartSql.Options/) |
| [SmartSql.Bulk](https://www.nuget.org/packages/SmartSql.Bulk/) | [![SmartSql.Bulk](https://img.shields.io/nuget/v/SmartSql.Bulk.svg)](https://www.nuget.org/packages/SmartSql.Bulk/)  | [![SmartSql.Bulk](https://img.shields.io/nuget/dt/SmartSql.Bulk.svg)](https://www.nuget.org/packages/SmartSql.Bulk/) |
| [SmartSql.Bulk.SqlServer](https://www.nuget.org/packages/SmartSql.Bulk.SqlServer/) | [![SmartSql.Bulk.SqlServer](https://img.shields.io/nuget/v/SmartSql.Bulk.SqlServer.svg)](https://www.nuget.org/packages/SmartSql.Bulk.SqlServer/)  | [![SmartSql.Bulk.SqlServer](https://img.shields.io/nuget/dt/SmartSql.Bulk.SqlServer.svg)](https://www.nuget.org/packages/SmartSql.Bulk.SqlServer/) |
| [SmartSql.Bulk.PostgreSql](https://www.nuget.org/packages/SmartSql.Bulk.PostgreSql/) | [![SmartSql.Bulk.PostgreSql](https://img.shields.io/nuget/v/SmartSql.Bulk.PostgreSql.svg)](https://www.nuget.org/packages/SmartSql.Bulk.PostgreSql/)  | [![SmartSql.Bulk.PostgreSql](https://img.shields.io/nuget/dt/SmartSql.Bulk.PostgreSql.svg)](https://www.nuget.org/packages/SmartSql.Bulk.PostgreSql/) 
| [SmartSql.Bulk.MySql](https://www.nuget.org/packages/SmartSql.Bulk.MySql/) | [![SmartSql.Bulk.MySql](https://img.shields.io/nuget/v/SmartSql.Bulk.MySql.svg)](https://www.nuget.org/packages/SmartSql.Bulk.MySql/)  | [![SmartSql.Bulk.MySql](https://img.shields.io/nuget/dt/SmartSql.Bulk.MySql.svg)](https://www.nuget.org/packages/SmartSql.Bulk.MySql/) |
| [SmartSql.Bulk.MySqlConnector](https://www.nuget.org/packages/SmartSql.Bulk.MySqlConnector/) | [![SmartSql.Bulk.MySqlConnector](https://img.shields.io/nuget/v/SmartSql.Bulk.MySqlConnector.svg)](https://www.nuget.org/packages/SmartSql.Bulk.MySqlConnector/)  | [![SmartSql.Bulk.MySqlConnector](https://img.shields.io/nuget/dt/SmartSql.Bulk.MySqlConnector.svg)](https://www.nuget.org/packages/SmartSql.Bulk.MySqlConnector/) |

## 示例项目

>[SmartSql.Sample.AspNetCore](https://github.com/dotnetcore/SmartSql/tree/master/sample/SmartSql.Sample.AspNetCore)

## 技术交流

点击链接加入QQ群【SmartSql 官方交流群】：[604762592](https://jq.qq.com/?_wv=1027&k=5Sy8Ahw)