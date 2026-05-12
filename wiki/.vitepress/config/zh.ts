import { DefaultTheme } from 'vitepress'

export const zh: DefaultTheme.Config = {
  label: '中文',
  lang: 'zh-CN',
  title: 'SmartSql',
  description: '受 MyBatis 启发的 .NET ORM —— 使用 XML 管理 SQL，支持读写分离、缓存与动态仓储',
  themeConfig: {
    nav: [
      { text: '指南', link: '/zh/guide/' },
      { text: '架构', link: '/zh/architecture/' },
      { text: '扩展', link: '/zh/extensions/' },
      { text: 'API', link: '/zh/api/' },
      { text: '入门指南', link: '/zh/onboarding/' },
      {
        text: 'v4.1',
        items: [
          { text: '更新日志', link: '/zh/guide/changelog' },
          { text: '贡献指南', link: '/zh/building/contributing' },
        ],
      },
    ],
    sidebar: {
      '/zh/guide/': [
        {
          text: '快速开始',
          items: [
            { text: '介绍', link: '/zh/guide/' },
            { text: '快速上手', link: '/zh/guide/quick-start' },
            { text: '配置', link: '/zh/guide/configuration' },
            { text: 'XML SQL 映射', link: '/zh/guide/xml-sql-maps' },
            { text: '更新日志', link: '/zh/guide/changelog' },
          ],
        },
      ],
      '/zh/architecture/': [
        {
          text: '架构',
          items: [
            { text: '概览', link: '/zh/architecture/' },
            { text: '中间件管道', link: '/zh/architecture/middleware-pipeline' },
            { text: 'XML 标签系统', link: '/zh/architecture/xml-tags' },
            { text: '数据源与读写分离', link: '/zh/architecture/datasource' },
            { text: '缓存', link: '/zh/architecture/caching' },
            { text: '反序列化', link: '/zh/architecture/deserialization' },
            { text: '诊断', link: '/zh/architecture/diagnostics' },
          ],
        },
      ],
      '/zh/extensions/': [
        {
          text: '扩展',
          items: [
            { text: '概览', link: '/zh/extensions/' },
            { text: '动态仓储', link: '/zh/extensions/dy-repository' },
            { text: 'DI 集成', link: '/zh/extensions/di-extension' },
            { text: 'Options 模式', link: '/zh/extensions/options' },
            { text: 'AOP 事务', link: '/zh/extensions/aop' },
            { text: '批量插入', link: '/zh/extensions/bulk-insert' },
            { text: '类型处理器', link: '/zh/extensions/type-handlers' },
            { text: 'Redis 缓存', link: '/zh/extensions/redis-cache' },
            { text: '缓存同步', link: '/zh/extensions/cache-sync' },
            { text: 'InvokeSync 与消息队列', link: '/zh/extensions/invoke-sync' },
            { text: '脚本标签', link: '/zh/extensions/script-tag' },
            { text: 'Oracle 支持', link: '/zh/extensions/oracle' },
          ],
        },
      ],
      '/zh/api/': [
        {
          text: 'API 参考',
          items: [
            { text: '概览', link: '/zh/api/' },
            { text: '核心接口', link: '/zh/api/core-interfaces' },
            { text: '配置 API', link: '/zh/api/configuration' },
            { text: '中间件 API', link: '/zh/api/middleware' },
          ],
        },
      ],
      '/zh/building/': [
        {
          text: '构建',
          items: [
            { text: '构建与 CI', link: '/zh/building/' },
            { text: '贡献指南', link: '/zh/building/contributing' },
            { text: '发布', link: '/zh/building/publishing' },
          ],
        },
      ],
      '/zh/onboarding/': [
        {
          text: '入门指南',
          collapsed: false,
          items: [
            { text: '贡献者指南', link: '/zh/onboarding/contributor' },
            { text: '高级工程师指南', link: '/zh/onboarding/staff-engineer' },
            { text: '管理层指南', link: '/zh/onboarding/executive' },
            { text: '产品经理指南', link: '/zh/onboarding/product-manager' },
          ],
        },
      ],
    },
    socialLinks: [
      { icon: 'github', link: 'https://github.com/dotnetcore/SmartSql' },
    ],
    footer: {
      message: '基于 MIT 许可证发布。',
      copyright: 'Copyright 2016-present Ahoo Wang',
    },
    editLink: {
      pattern: 'https://github.com/dotnetcore/SmartSql/edit/master/wiki/:path',
      text: '在 GitHub 上编辑此页面',
    },
    search: {
      provider: 'local',
      options: {
        translations: {
          button: {
            buttonText: '搜索',
            buttonAriaLabel: '搜索文档',
          },
          modal: {
            displayDetails: '显示详情',
            resetButtonTitle: '清除查询条件',
            backButtonTitle: '返回',
            noResultsText: '没有找到相关结果',
            footer: {
              selectText: '选择',
              navigateText: '切换',
              closeText: '关闭',
            },
          },
        },
      },
    },
    outline: {
      label: '页面导航',
    },
    lastUpdated: {
      text: '最后更新于',
    },
    docFooter: {
      prev: '上一页',
      next: '下一页',
    },
  },
}
