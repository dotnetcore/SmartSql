import { DefaultTheme } from 'vitepress'

export const en: DefaultTheme.Config = {
  label: 'English',
  lang: 'en',
  title: 'SmartSql',
  description: '.NET ORM inspired by MyBatis — XML-managed SQL with read/write splitting, caching, and dynamic repositories',
  themeConfig: {
    nav: [
      { text: 'Guide', link: '/guide/' },
      { text: 'Architecture', link: '/architecture/' },
      { text: 'Extensions', link: '/extensions/' },
      { text: 'API', link: '/api/' },
      { text: 'Onboarding', link: '/onboarding/' },
      {
        text: 'v4.1',
        items: [
          { text: 'Changelog', link: '/guide/changelog' },
          { text: 'Contributing', link: '/building/contributing' },
        ],
      },
    ],
    sidebar: {
      '/guide/': [
        {
          text: 'Getting Started',
          items: [
            { text: 'Introduction', link: '/guide/' },
            { text: 'Quick Start', link: '/guide/quick-start' },
            { text: 'Configuration', link: '/guide/configuration' },
            { text: 'XML SQL Maps', link: '/guide/xml-sql-maps' },
            { text: 'Changelog', link: '/guide/changelog' },
          ],
        },
      ],
      '/architecture/': [
        {
          text: 'Architecture',
          items: [
            { text: 'Overview', link: '/architecture/' },
            { text: 'Middleware Pipeline', link: '/architecture/middleware-pipeline' },
            { text: 'XML Tag System', link: '/architecture/xml-tags' },
            { text: 'DataSource & R/W Splitting', link: '/architecture/datasource' },
            { text: 'Caching', link: '/architecture/caching' },
            { text: 'Deserialization', link: '/architecture/deserialization' },
            { text: 'Diagnostics', link: '/architecture/diagnostics' },
          ],
        },
      ],
      '/extensions/': [
        {
          text: 'Extensions',
          items: [
            { text: 'Overview', link: '/extensions/' },
            { text: 'Dynamic Repository', link: '/extensions/dy-repository' },
            { text: 'DI Integration', link: '/extensions/di-extension' },
            { text: 'Options Pattern', link: '/extensions/options' },
            { text: 'AOP Transactions', link: '/extensions/aop' },
            { text: 'Bulk Insert', link: '/extensions/bulk-insert' },
            { text: 'Type Handlers', link: '/extensions/type-handlers' },
            { text: 'Redis Cache', link: '/extensions/redis-cache' },
            { text: 'Cache Sync', link: '/extensions/cache-sync' },
            { text: 'InvokeSync & Messaging', link: '/extensions/invoke-sync' },
            { text: 'Script Tag', link: '/extensions/script-tag' },
            { text: 'Oracle Support', link: '/extensions/oracle' },
          ],
        },
      ],
      '/api/': [
        {
          text: 'API Reference',
          items: [
            { text: 'Overview', link: '/api/' },
            { text: 'Core Interfaces', link: '/api/core-interfaces' },
            { text: 'Configuration API', link: '/api/configuration' },
            { text: 'Middleware API', link: '/api/middleware' },
          ],
        },
      ],
      '/building/': [
        {
          text: 'Building',
          items: [
            { text: 'Build & CI', link: '/building/' },
            { text: 'Contributing', link: '/building/contributing' },
            { text: 'Publishing', link: '/building/publishing' },
          ],
        },
      ],
      '/onboarding/': [
        {
          text: 'Onboarding',
          collapsed: false,
          items: [
            { text: 'Contributor Guide', link: '/onboarding/contributor' },
            { text: 'Staff Engineer Guide', link: '/onboarding/staff-engineer' },
            { text: 'Executive Guide', link: '/onboarding/executive' },
            { text: 'Product Manager Guide', link: '/onboarding/product-manager' },
          ],
        },
      ],
    },
    socialLinks: [
      { icon: 'github', link: 'https://github.com/dotnetcore/SmartSql' },
    ],
    footer: {
      message: 'Released under the MIT License.',
      copyright: 'Copyright 2016-present Ahoo Wang',
    },
    editLink: {
      pattern: 'https://github.com/dotnetcore/SmartSql/edit/master/wiki/:path',
      text: 'Edit this page on GitHub',
    },
    search: {
      provider: 'local',
      options: {
        translations: {
          button: {
            buttonText: 'Search',
            buttonAriaLabel: 'Search',
          },
          modal: {
            displayDetails: 'Display detailed list',
            resetButtonTitle: 'Reset search',
            backButtonTitle: 'Close search',
            noResultsText: 'No results for',
            footer: {
              selectText: 'to select',
              navigateText: 'to navigate',
              closeText: 'to close',
            },
          },
        },
      },
    },
  },
}
