# Milkora Desktop (Electron.NET host)

Packages the whole app as a **single desktop process**: a local ASP.NET Core (Kestrel)
host that serves the Angular UI **and** the API **and** the Dotmim.Sync endpoint, plus a
background sync engine — all wrapped in a Chromium window by Electron.NET.

```
┌─────────────────────── Electron window ───────────────────────┐
│  Angular UI  ──HTTP(same origin)──►  local Kestrel            │
│                                       ├─ /            (wwwroot: Angular)
│                                       ├─ /api/*       (API controllers)
│                                       ├─ /api/sync    (Dotmim.Sync server)
│                                       └─ /api/desktop/sync  (UI ↔ engine bridge)
│  Background: MilkoraSyncManager  ──►  local SQLite  ⇄  central MS SQL │
└───────────────────────────────────────────────────────────────┘
```

## What runs where
- **UI** is the production Angular build, copied into `wwwroot/`.
- **API** controllers are reused from `Milkora.API` via `AddApplicationPart` (no duplication).
- **Sync engine** (`Milkora.SyncClient.MilkoraSyncManager`) runs as a hosted service
  (`SyncHostedService`), keeping a local `dairy_local.db` in sync with central SQL.
  Its target URL defaults to the host's own `/api/sync` when `Sync:ServerSyncUrl` is blank.
- The UI's **sync chip / "Sync Now"** talk to `DesktopSyncController` (`/api/desktop/sync`).
  In the plain web build those routes 404 and the chip falls back to online/offline.

## Prerequisites
- .NET 8 SDK, Node.js 22+.
- Electron.NET CLI: `dotnet tool install ElectronNET.CLI -g`
- The installed `electronize` targets .NET 6; if that runtime is absent, roll it forward:
  `set DOTNET_ROLL_FORWARD=LatestMajor` (Windows) before any `electronize` command.

## 1) Refresh the UI into wwwroot (after any Angular change)
```bash
cd ../../Milkora.Client
ng build --configuration production
# copy dist/milkora.client/browser/* into ../src/Milkora.Desktop/wwwroot/
```
(or run `pwsh ./refresh-ui.ps1` from this folder.)

## 2) Run as a plain web host (fast dev / debugging, no Electron)
```bash
dotnet run --project Milkora.Desktop.csproj
# then open the shown http://localhost:<port>
```

## 3) Run as the Electron desktop app
```bash
set DOTNET_ROLL_FORWARD=LatestMajor
electronize start
```
First run downloads the Electron binaries (~100–200 MB) — expect a slow first launch.

## 4) Build a Windows installer
```bash
set DOTNET_ROLL_FORWARD=LatestMajor
electronize build /target win
# output: bin/Desktop/
```
Installer size is ~100–200 MB (Chromium is bundled) — expected for Electron.

## Gotchas
- **`InvariantGlobalization` must be OFF** in this project — `Microsoft.Data.Sqlite`
  (the sync engine) throws "Globalization Invariant Mode is not supported" otherwise.
- Config: `appsettings.json` → `ConnectionStrings:MilkoraDb` (central SQL) and the `Sync`
  section (local db path, interval, retries). Leave `ServerSyncUrl` blank to self-target.
- This build reads/writes **central SQL** while online and keeps a synced local SQLite copy.
  Serving reads *from* local SQLite when fully offline needs a SQLite data-access layer
  (a separate `Milkora.Infrastructure.Sqlite`) — a planned follow-up.
