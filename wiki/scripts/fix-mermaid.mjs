#!/usr/bin/env node

/**
 * Automated Mermaid syntax validator and fixer for SmartSql wiki.
 *
 * Fixes:
 * 1. Replace `<br/>` with `<br>` (Vue compiler compatibility)
 * 2. Replace light-mode inline styles with dark-mode equivalents
 * 3. Add `autonumber` to sequenceDiagram blocks, remove duplicates
 * 4. Remove `style "quoted name"` directives on subgraphs (parse errors)
 * 5. Remove `linkStyle` directives (index out of bounds errors)
 * 6. Remove `style` directives inside sequenceDiagram blocks (not supported)
 * 7. Fix quoted subgraph names to use `subgraph id ["Label"]` syntax
 * 8. Clean up blank lines left by removals
 * 9. Remove parentheses from quadrantChart labels (lexical error)
 */

import { readFileSync, writeFileSync, readdirSync, statSync } from 'fs'
import { join, extname } from 'path'

const MD_EXTENSIONS = new Set(['.md'])
let counter = 0

function walkDir(dir) {
  const files = []
  for (const entry of readdirSync(dir)) {
    const fullPath = join(dir, entry)
    const stat = statSync(fullPath)
    if (stat.isDirectory() && !entry.startsWith('.') && entry !== 'node_modules') {
      files.push(...walkDir(fullPath))
    } else if (MD_EXTENSIONS.has(extname(entry))) {
      files.push(fullPath)
    }
  }
  return files
}

function fixMermaidBlock(block) {
  let fixed = block
  let issues = []
  const firstLine = fixed.trim().split('\n')[0].trim()
  const isSequence = firstLine === 'sequenceDiagram'
  const isQuadrant = firstLine === 'quadrantChart'

  // Fix: Replace <br/> with <br>
  const brSlashCount = (fixed.match(/<br\/>/g) || []).length
  if (brSlashCount > 0) {
    fixed = fixed.replace(/<br\/>/g, '<br>')
    issues.push(`Replaced ${brSlashCount}x <br/> with <br>`)
  }

  // Fix: Convert quoted subgraph names to ID-based syntax
  // subgraph "Some Name" → subgraph sg_N ["Some Name"]
  const subgraphRegex = /subgraph\s+"([^"]+)"/g
  let subMatch
  const subNameToId = new Map()
  let tempFixed = fixed
  while ((subMatch = subgraphRegex.exec(fixed)) !== null) {
    const quotedName = subMatch[1]
    const id = 'sg_' + (++counter)
    subNameToId.set(quotedName, id)
    tempFixed = tempFixed.replace(subMatch[0], `subgraph ${id} ["${quotedName}"]`)
  }
  if (subNameToId.size > 0) {
    fixed = tempFixed
    issues.push(`Fixed ${subNameToId.size}x quoted subgraph names to ID-based syntax`)
  }

  // Fix: Remove style "quoted name" directives
  const quotedStyleRegex = /^\s*style\s+"[^"]+"\s+fill:.*$/gm
  const quotedStyleCount = (fixed.match(quotedStyleRegex) || []).length
  if (quotedStyleCount > 0) {
    fixed = fixed.replace(quotedStyleRegex, '')
    issues.push(`Removed ${quotedStyleCount}x style "quoted name" directives`)
  }

  // Fix: Remove ALL style directives from sequenceDiagram (not supported)
  if (isSequence) {
    const styleRegex = /^\s*style\s+\S+\s+fill:.*$/gm
    const styleCount = (fixed.match(styleRegex) || []).length
    if (styleCount > 0) {
      fixed = fixed.replace(styleRegex, '')
      issues.push(`Removed ${styleCount}x style directives from sequenceDiagram`)
    }
  }

  // Fix: Remove linkStyle directives
  const linkStyleRegex = /^\s*linkStyle\s+.*$/gm
  const linkStyleCount = (fixed.match(linkStyleRegex) || []).length
  if (linkStyleCount > 0) {
    fixed = fixed.replace(linkStyleRegex, '')
    issues.push(`Removed ${linkStyleCount}x linkStyle directives`)
  }

  // Fix: sequenceDiagram - handle autonumber
  if (isSequence) {
    // Remove all autonumber lines first
    fixed = fixed.replace(/^\s*autonumber\s*$/gm, '')
    // Prepend single autonumber after the diagram type line
    fixed = fixed.replace(/^(sequenceDiagram)\n*/, '$1\nautonumber\n')
    issues.push('Fixed autonumber in sequenceDiagram')
  }

  // Fix: quadrantChart - remove parentheses from labels (lexical error)
  if (isQuadrant) {
    const parenRegex = /^(\s+[\w\s]+)\(([^)]+)\)(:\s*\[[\d., ]+\])$/gm
    let parenMatch
    let parenFixed = fixed
    let parenCount = 0
    while ((parenMatch = parenRegex.exec(fixed)) !== null) {
      parenFixed = parenFixed.replace(parenMatch[0], parenMatch[1] + parenMatch[2] + parenMatch[3])
      parenCount++
    }
    if (parenCount > 0) {
      fixed = parenFixed
      issues.push(`Removed ${parenCount}x parentheses from quadrantChart labels`)
    }
  }

  // Clean up excessive blank lines
  fixed = fixed.replace(/\n{3,}/g, '\n\n')

  return { fixed, issues }
}

function fixMermaidBlocks(content) {
  let allIssues = []

  const result = content.replace(/```mermaid\n([\s\S]*?)```/g, (match, block) => {
    const { fixed, issues } = fixMermaidBlock(block)
    allIssues.push(...issues)
    return '```mermaid\n' + fixed + '```'
  })

  // Fix: light-mode colors globally
  const lightColorMap = {
    'fill:#fff': 'fill:#2d333b',
    'fill:#ffffff': 'fill:#2d333b',
    'fill:white': 'fill:#2d333b',
    'stroke:#000': 'stroke:#6d5dfc',
    'stroke:#333': 'stroke:#8b949e',
    'color:#000': 'color:#e6edf3',
    'color:#333': 'color:#e6edf3',
    'fill:#f9f9f9': 'fill:#161b22',
    'fill:#fafafa': 'fill:#161b22',
    'fill:#f5f5f5': 'fill:#21262d',
  }

  let finalResult = result
  for (const [light, dark] of Object.entries(lightColorMap)) {
    const count = (finalResult.match(new RegExp(light, 'gi')) || []).length
    if (count > 0) {
      finalResult = finalResult.replace(new RegExp(light, 'gi'), dark)
      allIssues.push(`Replaced ${count}x ${light} with ${dark}`)
    }
  }

  return { fixed: finalResult, issues: allIssues }
}

// Main execution
const wikiDir = join(import.meta.dirname, '..')
const files = walkDir(wikiDir)

let totalIssues = 0
let filesFixed = 0

for (const file of files) {
  const content = readFileSync(file, 'utf-8')
  const { fixed, issues } = fixMermaidBlocks(content)

  if (issues.length > 0) {
    console.log(`\n${file.replace(wikiDir, '.')}:`)
    for (const issue of issues) {
      console.log(`  - ${issue}`)
    }
    writeFileSync(file, fixed, 'utf-8')
    filesFixed++
    totalIssues += issues.length
  }
}

console.log(`\n--- Summary ---`)
console.log(`Files scanned: ${files.length}`)
console.log(`Files fixed: ${filesFixed}`)
console.log(`Issues found and fixed: ${totalIssues}`)

if (totalIssues === 0) {
  console.log('All Mermaid blocks look good!')
}

process.exit(0)
