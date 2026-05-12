---
layout: home

hero:
  name: SmartSql
  text: .NET ORM Inspired by MyBatis
  image:
    src: /SmartSql.png
    alt: SmartSql
  tagline: XML-managed SQL with read/write splitting, caching, dynamic repositories, and diagnostics — simple, efficient, high-performance.
  actions:
    - theme: brand
      text: Get Started
      link: /guide/
    - theme: alt
      text: Architecture
      link: /architecture/
    - theme: alt
      text: View on GitHub
      link: https://github.com/dotnetcore/SmartSql

features:
  - icon: <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M14.5 2H6a2 2 0 00-2 2v16a2 2 0 002 2h12a2 2 0 002-2V7.5L14.5 2z"/><polyline points="14 2 14 8 20 8"/><path d="M8 13h2"/><path d="M8 17h2"/><path d="M14 13h2"/><path d="M14 17h2"/></svg>
    title: XML SQL Management
    details: Define and manage SQL statements in XML files with dynamic tags for conditional SQL construction — IsNotEmpty, IsEqual, Switch, Where, Set, For, and more.
  - icon: <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M12 2L2 7l10 5 10-5-10-5z"/><path d="M2 17l10 5 10-5"/><path d="M2 12l10 5 10-5"/></svg>
    title: Read/Write Splitting
    details: Configure read and write data sources declaratively in XML. Automatic weighted load balancing across multiple read replicas.
  - icon: <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="2" y="3" width="20" height="14" rx="2"/><path d="M8 21h8"/><path d="M12 17v4"/><path d="M6 7h4"/><path d="M6 11h12"/></svg>
    title: Middleware Pipeline
    details: Modular execution pipeline with initializer, statement preparation, caching, transaction, data source filtering, command execution, and result handling.
  - icon: <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M16 21v-2a4 4 0 00-4-4H6a4 4 0 00-4-4v2"/><circle cx="9" cy="7" r="4"/><path d="M22 21v-2a4 4 0 00-3-3.87"/><path d="M16 3.13a4 4 0 010 7.75"/></svg>
    title: Dynamic Repository
    details: Auto-implement repository interfaces via IL emit. Interface methods map to SQL statements by naming convention — zero boilerplate.
  - icon: <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M22 12h-4l-3 9L9 3l-3 9H2"/></svg>
    title: Built-in Caching
    details: LRU and FIFO in-memory caches plus Redis distributed cache. Configurable flush intervals and FlushOnExecute for automatic invalidation.
  - icon: <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"/><path d="M12 6v6l4 2"/></svg>
    title: Diagnostics & Monitoring
    details: Built-in DiagnosticSource events for command execution, session lifecycle, and errors. Integrates with APM tools like SkyWalking.
---
