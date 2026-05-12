import { defineConfig } from 'vitepress'
import { en } from './en'
import { zh } from './zh'

export default defineConfig({
  title: 'SmartSql',
  description: '.NET ORM inspired by MyBatis — XML-managed SQL with read/write splitting, caching, and dynamic repositories',
  lastUpdated: true,
  cleanUrls: true,
  ignoreDeadLinks: [
    /localhost/,
  ],
  head: [
    ['link', { rel: 'icon', type: 'image/svg+xml', href: '/logo.svg' }],
    ['script', { async: '', src: 'https://www.googletagmanager.com/gtag/js?id=G-ZFQNQXMXS3' }],
    ['script', {}, `window.dataLayer = window.dataLayer || []; function gtag(){dataLayer.push(arguments);} gtag('js', new Date()); gtag('config', 'G-ZFQNQXMXS3');`],
  ],
  locales: {
    root: {
      ...en,
    },
    zh: {
      ...zh,
    },
  },
  sitemap: {
    hostname: 'https://wiki.smartsql.net',
  },
  markdown: {
    lineNumbers: true,
  },
  vite: {
    plugins: [],
  },
})
