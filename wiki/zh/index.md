---
layout: home

hero:
  name: SmartSql
  text: 受 MyBatis 启发的 .NET ORM
  image:
    src: /SmartSql.png
    alt: SmartSql
  tagline: 使用 XML 管理 SQL，支持读写分离、缓存、动态仓储与诊断 —— 简单、高效、高性能。
  actions:
    - theme: brand
      text: 快速开始
      link: /zh/guide/
    - theme: alt
      text: 架构设计
      link: /zh/architecture/
    - theme: alt
      text: 在 GitHub 上查看
      link: https://github.com/dotnetcore/SmartSql

features:
  - icon: <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M14.5 2H6a2 2 0 00-2 2v16a2 2 0 002 2h12a2 2 0 002-2V7.5L14.5 2z"/><polyline points="14 2 14 8 20 8"/><path d="M8 13h2"/><path d="M8 17h2"/><path d="M14 13h2"/><path d="M14 17h2"/></svg>
    title: XML SQL 管理
    details: 在 XML 文件中定义和管理 SQL 语句，使用动态标签实现条件 SQL 构建 —— 支持 IsNotEmpty、IsEqual、Switch、Where、Set、For 等丰富标签。
  - icon: <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M12 2L2 7l10 5 10-5-10-5z"/><path d="M2 17l10 5 10-5"/><path d="M2 12l10 5 10-5"/></svg>
    title: 读写分离
    details: 在 XML 中声明式地配置读写数据源，自动在多个读副本之间进行加权负载均衡。
  - icon: <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="2" y="3" width="20" height="14" rx="2"/><path d="M8 21h8"/><path d="M12 17v4"/><path d="M6 7h4"/><path d="M6 11h12"/></svg>
    title: 中间件管道
    details: 模块化执行管道，包含初始化、语句准备、缓存、事务、数据源过滤、命令执行和结果处理等中间件。
  - icon: <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M16 21v-2a4 4 0 00-4-4H6a4 4 0 00-4-4v2"/><circle cx="9" cy="7" r="4"/><path d="M22 21v-2a4 4 0 00-3-3.87"/><path d="M16 3.13a4 4 0 010 7.75"/></svg>
    title: 动态仓储
    details: 通过 IL Emit 自动生成仓储接口实现。接口方法通过命名约定映射到 SQL 语句 —— 零样板代码。
  - icon: <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M22 12h-4l-3 9L9 3l-3 9H2"/></svg>
    title: 内置缓存
    details: LRU 和 FIFO 内置缓存加上 Redis 分布式缓存。可配置的刷新间隔和 FlushOnExecute 自动失效机制。
  - icon: <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"/><path d="M12 6v6l4 2"/></svg>
    title: 诊断与监控
    details: 内置 DiagnosticSource 事件，涵盖命令执行、会话生命周期和错误。可与 SkyWalking 等 APM 工具集成。
---
