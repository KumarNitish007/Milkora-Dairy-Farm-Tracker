# 🐄 Milkora Dairy Farm Tracker

An **offline-first** dairy farm management application — track animals, milk production,
sales, expenses, income, health, breeding, feed inventory and reports.

One Angular codebase, three ways to run it:

| Mode | How | Data |
|------|-----|------|
| **Web** | Angular served by the API (or any static host) | talks to the API directly; PWA service worker for offline shell + cached reads |
| **Desktop** | Electron wraps a local .NET host (`Milkora.Desktop`) | one local process serves UI + API + runs a background SQLite sync engine |
| **Mobile** | the same responsive web app in a phone browser | installable PWA |

Single user, no login. Currency ₹ (INR).

---

## 1. Tech stack

| Layer | Technology |
|-------|-----------|
| Frontend | Angular 19 (standalone components, Signals), responsive top **nav bar** UI |
| Desktop wrapper | ElectronNET.API (.NET 8) |
| Backend | ASP.NET Core 8 Web API |
| Data access | **ADO.NET + stored procedures only** (no Entity Framework) |
| Central DB | MS SQL Server (LocalDB in dev) |
| Local DB | SQLite (per device) |
| Sync engine | Dotmim.Sync 1.3.0 — bi-directional SQLite ↔ SQL Server, **ServerWins** |
| Web offline | Angular PWA service worker |
| Validation | FluentValidation |
| Tests | xUnit + NSubstitute + FluentAssertions + `WebApplicationFactory` |

**Design rules:** GUID (`UNIQUEIDENTIFIER`) primary keys everywhere (prevents offline
collisions); every DB operation goes through a stored procedure; all parameters bound with
`SqlParameter` (injection-safe); UTC timestamps; unified `ApiResponse<T>` envelope.

---

## 2. Solution structure (Clean Architecture)

```
Milkora.DairyFarm/
├── Milkora.sln
├── SQL/                          # DB scripts (run in order 00 → 03)
├── src/
│   ├── Milkora.Domain/           # entities, enums/constants, exceptions, repo interfaces — zero deps
│   ├── Milkora.Application/       # DTOs + manual mappers, service interfaces/impls, validators, ApiResponse
│   ├── Milkora.Infrastructure/    # ADO.NET repositories + IDbConnectionFactory (implements Domain interfaces)
│   ├── Milkora.API/               # controllers, middleware, ValidationFilter, DI, Dotmim.Sync server endpoint
│   ├── Milkora.Sync/              # shared Dotmim.Sync setup (the 8 tables, computed cols excluded)
│   ├── Milkora.SyncClient/        # device sync engine (SQLite ↔ API), triggers + status + retry
│   ├── Milkora.SyncClient.Runner/ # console host / demo for the sync engine
│   └── Milkora.Desktop/           # Electron + local Kestrel host (serves UI + API + runs the sync engine)
├── Milkora.Client/               # Angular 19 app (core / shared / layout / features)
└── tests/
    ├── Milkora.Application.Tests/ # unit tests (mocked repos, validators, mapping)
    └── Milkora.Api.Tests/         # integration tests (real API + LocalDB)
```

**Dependency flow:** API → Application → Domain ← Infrastructure (dependency inversion).

---

## 3. Prerequisites

- **.NET 8 SDK** (the .NET 9 SDK also builds these projects; the ASP.NET Core **8 runtime** must be installed to run)
- **Node.js 22+** and **Angular CLI 19** (`npm i -g @angular/cli@19`)
- **SQL Server** or **LocalDB** (`(localdb)\MSSQLLocalDB`)
- For desktop packaging only: **ElectronNET CLI** — `dotnet tool install ElectronNET.CLI -g`

---

## 4. Setup & run

### Step 1 — Database
```bash
cd SQL
sqlcmd -S "(localdb)\MSSQLLocalDB" -i 00_CreateDatabase.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -i 01_Tables.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -i 02_StoredProcedures.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -i 03_SeedData.sql   # optional sample data
```
Connection string lives in each host's `appsettings.json` under `ConnectionStrings:MilkoraDb`.

### Step 2 — API (backend)
```bash
cd src/Milkora.API
dotnet run
```
- HTTP `http://localhost:5150`, HTTPS `https://localhost:7150`
- Swagger UI: `https://localhost:7150/swagger`
- Sync endpoint: `POST /api/sync`

### Step 3 — Angular app (web, dev)
```bash
cd Milkora.Client
npm install      # first time only
ng serve
```
- App: `http://localhost:4200`
- `proxy.conf.json` forwards `/api` → `https://localhost:7150` (no CORS/cert friction).

### Step 4 — Desktop app
Build the Angular app and copy it into the host's `wwwroot`, then run the host:
```bash
# 1. production build of the UI
cd Milkora.Client
ng build --configuration production

# 2. copy build output into the desktop host
#    from: Milkora.Client/dist/milkora.client/browser/*
#    to:   src/Milkora.Desktop/wwwroot/

# 3a. run as a plain local host (no Electron window) — good for testing
cd ../src/Milkora.Desktop
dotnet run                    # serves UI + API + sync engine on its Kestrel port

# 3b. OR launch the real Electron desktop window (requires ElectronNET CLI)
electronize start             # dev
electronize build /target win # produces an installer under bin/Desktop
```
The desktop host is self-contained: it serves the Angular UI, the API, the sync
endpoint, and runs the background **SQLite sync engine** — all in one process. The UI's
"Sync Now" button and status chip are wired to the real engine via `/api/desktop/sync`.

### Step 5 — Standalone sync engine (optional)
```bash
cd src/Milkora.SyncClient.Runner
dotnet run -- demo   # download seed → write locally → upload; proves offline-first
dotnet run           # background sync loop (initial + timer + on reconnect)
```

### Step 6 — Tests
```bash
dotnet test Milkora.sln
```
28 unit + 5 integration tests (integration needs LocalDB with `MilkoraDB` present).

---

## 5. Modules (12)

Dashboard · Animals · Milk Log · Sales · Expenses · Income · Health · Breeding ·
Feed Inventory · Reports · Reminders · Settings.

---

## 6. Offline-first flow

1. User action → UI writes locally (SQLite on desktop) → instant, no internet needed.
2. The sync engine runs in the background: on start, every N minutes, on reconnect, or manual.
3. When online: upload local changes → download server changes → resolve conflicts (**ServerWins**).
4. The nav-bar chip shows **Synced / Syncing / Pending / Offline / Error**.

GUID primary keys mean two devices creating records offline never collide.

---

## 7. Gotchas & notes

- **Desktop host must NOT use `InvariantGlobalization`** — Microsoft.Data.Sqlite (the sync
  engine) requires ICU globalization. It's disabled in `Milkora.Desktop.csproj`.
- The Dotmim.Sync **web server requires ASP.NET Session** (`AddSession` + `UseSession`) —
  already wired in the API and desktop hosts.
- **Dotmim.Sync is .NET-only** — it runs on the device (desktop). The browser web app can't
  run it and talks to the API directly; the PWA service worker handles web offline instead.
- Dotmim.Sync version is **1.3.0** (there is no 1.4.0 yet).
- The **.NET 9 SDK builds these `net8.0` projects** fine; you need the ASP.NET Core 8 runtime to run.
- Provisioning adds `*_tracking` tables + `scope_info` tables to `MilkoraDB` — that's expected sync infrastructure.
- The service worker is only active in **production** Angular builds (`enabled: !isDevMode()`).

---

## 8. Status

All spec steps (1–8) are complete and verified: DB, API, sync endpoint, Angular UI,
SQLite sync client, Electron desktop host, PWA, and automated tests (33 passing).

Optional future work: a `Milkora.Infrastructure.Sqlite` data layer so the desktop serves
reads entirely from local SQLite (fully offline, not just online + sync cache).
