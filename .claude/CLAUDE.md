
You are an expert in TypeScript, Angular, and scalable web application development. You write functional, maintainable, performant, and accessible code following Angular and TypeScript best practices.

## TypeScript Best Practices

- Use strict type checking
- Prefer type inference when the type is obvious
- Avoid the `any` type; use `unknown` when type is uncertain

## Angular Best Practices

- Always use standalone components over NgModules
- Must NOT set `standalone: true` inside Angular decorators. It's the default in Angular v20+.
- Use signals for state management
- Implement lazy loading for feature routes
- Do NOT use the `@HostBinding` and `@HostListener` decorators. Put host bindings inside the `host` object of the `@Component` or `@Directive` decorator instead
- Use `NgOptimizedImage` for all static images.
  - `NgOptimizedImage` does not work for inline base64 images.

## Accessibility Requirements

- It MUST pass all AXE checks.
- It MUST follow all WCAG AA minimums, including focus management, color contrast, and ARIA attributes.

### Components

- Keep components small and focused on a single responsibility
- Use `input()` and `output()` functions instead of decorators
- Use `computed()` for derived state
- Set `changeDetection: ChangeDetectionStrategy.OnPush` in `@Component` decorator
- Prefer inline templates for small components
- Prefer Reactive forms instead of Template-driven ones
- Do NOT use `ngClass`, use `class` bindings instead
- Do NOT use `ngStyle`, use `style` bindings instead
- When using external templates/styles, use paths relative to the component TS file.

## State Management

- Use signals for local component state
- Use `computed()` for derived state
- Keep state transformations pure and predictable
- Do NOT use `mutate` on signals, use `update` or `set` instead

## Templates

- Keep templates simple and avoid complex logic
- Use native control flow (`@if`, `@for`, `@switch`) instead of `*ngIf`, `*ngFor`, `*ngSwitch`
- Use the async pipe to handle observables
- Do not assume globals like (`new Date()`) are available.

## Services

- Design services around a single responsibility
- Use the `providedIn: 'root'` option for singleton services
- Use the `inject()` function instead of constructor injection

## Project context: DevJourney (Phase 1 — frontend only)

A personal developer blog/learning platform. Full brief lives in the repo
owner's original spec; this is the condensed map for picking up mid-build.

**Where things live**
- `core/models/` — all TS interfaces (BlogPost, Category, Tag, Author,
  BlogImage, Pagination<T>, SearchResult, RoadmapItem, Resource,
  BlogContentBlock). Import from `core/models` (barrel).
- `core/constants/site-config.ts` — the ONLY place branding/nav/footer
  links should be defined. Never hard-code "DevJourney" or a nav label
  elsewhere.
- `layout/` — Header, Footer, PublicLayout (the public site chrome).
  Admin gets its own layout later (Step 9), not built yet.
- `shared/components/route-placeholder/` — the "coming soon" panel used by
  every not-yet-built route. Once a step is implemented, replace that
  route's `loadComponent` target in `app.routes.ts` with the real page and
  delete the placeholder usage — don't delete the component itself until
  nothing references it.
- `features/*/pages/` — one folder per route. Page components currently
  render `<dj-route-placeholder>` with an inline template; convert to
  `templateUrl` once there's real markup.

**Design system** — see `/docs/ui-reference-analysis.md` at the repo root
for the full rationale. Short version: tokens live in `src/styles.css`
under `@theme` (Tailwind v4, CSS-first config, no `tailwind.config.js`).
Colors: `ink` / `canvas` / `surface` / `border` are neutral; `accent`
(marigold) is reserved for the single highest-emphasis action on a screen;
`signal` (slate-indigo) is for secondary interactive elements. Fonts:
`font-display` (Space Grotesk, headings), `font-sans` (Source Sans 3,
body — this IS the default now), `font-mono` (JetBrains Mono, code +
metadata/badges). Reusable classes: `.btn-primary` / `.btn-accent` /
`.btn-secondary` / `.badge` / `.card` / `.link-quiet` in
`@layer components`. Don't write one-off hex values or reach for a second
accent color — extend the theme block instead.

**Status (Section 37 build order)**
- ✅ Steps 1–4: reference analysis, app shell, routing skeleton, models,
  header/footer, design tokens.
- ⬜ Step 5: Home page (hero → newsletter CTA, all Section 8–17 subsections).
- ⬜ Step 6: Blog listing (`/blog`).
- ⬜ Step 7: Blog detail (`/blog/:slug`).
- ⬜ Step 8: Categories/topics/search.
- ⬜ Step 9: Admin shell + layout.
- ⬜ Step 10: Rich text editor.
- ⬜ Step 11: Responsive polish pass.
- ⬜ Step 12: Final UI review against the reference site.

**Conventions specific to this repo**
- No component-level `.css` files — Tailwind utility classes only (matches
  how the rest of the author's projects are styled).
- Mock data will live in `features/*/services` as small in-memory arrays
  returning `Promise`/`Observable`/signals — not decided yet, decide at
  Step 5 and stay consistent afterward.
- `withComponentInputBinding()` is enabled — prefer binding route params
  (`:slug`, `:id`) straight to `input()` signals over `ActivatedRoute`
  snapshot digging.
