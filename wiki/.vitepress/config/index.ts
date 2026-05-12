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
  ],
  locales: {
    root: {
      ...en,
    },
    zh: {
      ...zh,
    },
  },
  markdown: {
    lineNumbers: true,
  },
  vite: {
    plugins: [],
  },
})
