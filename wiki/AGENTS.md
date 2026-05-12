# AGENTS.md — SmartSql Wiki

## Build & Run Commands

```bash
# Install dependencies
pnpm install

# Development server
pnpm dev

# Build static site
pnpm build

# Preview production build
pnpm preview

# Fix Mermaid diagram syntax issues
pnpm fix:mermaid
```

## Project Structure

```
wiki/
  .vitepress/
    config/
      index.ts      # Main VitePress config (imports en/zh locales)
      en.ts          # English locale config (nav, sidebar, footer)
      zh.ts          # Chinese locale config
      mermaid.ts     # Mermaid theme variables (reference)
    theme/
      index.ts       # Custom theme with vitepress-mermaid-renderer
      custom.css     # Dark-mode styling, brand colors, layout
  public/
    logo.svg         # Site favicon
  scripts/
    fix-mermaid.mjs  # Automated Mermaid syntax validator/fixer
  guide/             # Getting Started documentation
  architecture/      # Architecture deep-dives
  extensions/        # Extension packages documentation
  api/               # API reference
  building/          # Build, CI, publishing guides
  onboarding/        # Audience-tailored onboarding guides
  zh/                # Chinese translations (mirrors EN structure)
  index.md           # Home page (VitePress home layout)
```

## Content Conventions

### Mermaid Diagrams
- **Dark-mode colors required**: node fills `#2d333b`, borders `#6d5dfc`, text `#e6edf3`
- Subgraph backgrounds: `#161b22`, borders `#30363d`
- Lines: `#8b949e`
- Use `autonumber` in all `sequenceDiagram` blocks
- Do NOT use `<br/>` — use `<br>` (self-closing breaks Vue compiler)
- Every diagram must have a `<!-- Sources: file_path:line -->` comment

### Citations
- Format: `[file_path:line_number](https://github.com/dotnetcore/SmartSql/blob/master/file_path#Lline_number)`

### Frontmatter
Every page must have:
```yaml
---
title: Page Title
description: One-line description for SEO
---
```

### Chinese Translations
- All English pages must have Chinese counterparts in `zh/` directory
- Chinese sidebar paths prefixed with `/zh/`

## Documentation Sections

- `guide/` — Getting Started: Introduction, Quick Start, Configuration, XML SQL Maps, Changelog
- `architecture/` — Architecture: Overview, Middleware Pipeline, XML Tags, DataSource, Caching, Deserialization, Diagnostics
- `extensions/` — Extensions: 12 extension packages documentation
- `api/` — API Reference: Core Interfaces, Configuration API, Middleware API
- `building/` — Building: Build & CI, Contributing, Publishing
- `onboarding/` — Onboarding: Contributor, Staff Engineer, Executive, Product Manager guides

## Boundaries

- Do NOT delete generated wiki pages without explicit instruction
- Do NOT modify theme files (`custom.css`, `theme/index.ts`) without testing `pnpm build`
- Do NOT change sidebar structure in `en.ts`/`zh.ts` without updating both locales
- Always run `pnpm fix:mermaid` after adding or editing Mermaid diagrams
- Run `pnpm build` to verify the site compiles before committing

## Tech Stack

- **VitePress** ^1.6.4 — Static site generator
- **vitepress-mermaid-renderer** ^1.1.23 — Mermaid diagram rendering with zoom
- **pnpm** — Package manager
- **TypeScript** — Config files
