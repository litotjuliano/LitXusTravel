import { chromium } from 'playwright'
import path from 'path'
import { fileURLToPath } from 'url'
import { mkdirSync } from 'fs'

const __dirname = path.dirname(fileURLToPath(import.meta.url))
const OUT = path.join(__dirname, 'screenshots')
mkdirSync(OUT, { recursive: true })

const browser = await chromium.launch()
const ctx = await browser.newContext({ viewport: { width: 1440, height: 900 } })
const page = await ctx.newPage()

const pages = [
  { url: 'http://localhost:3001', name: '01-dashboard' },
  { url: 'http://localhost:3001/packages', name: '02-packages' },
  { url: 'http://localhost:3001/tenants', name: '03-tenants' },
  { url: 'http://localhost:3001/analytics', name: '04-analytics' },
  { url: 'http://localhost:3001/auth/login', name: '05-login' },
]

for (const { url, name } of pages) {
  await page.goto(url, { waitUntil: 'networkidle' })
  await page.waitForTimeout(1500)
  await page.screenshot({ path: path.join(OUT, `${name}.png`), fullPage: false })
  console.log(`✓ ${name}`)
}

await browser.close()
