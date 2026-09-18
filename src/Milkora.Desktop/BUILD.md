# Building the Milkora desktop installer

Produces a Windows `.exe` installer (Electron + bundled .NET 8 host) with the cow icon.

## Prerequisites
- .NET 8 SDK, Node.js 22
- ElectronNET CLI: `dotnet tool install ElectronNET.CLI -g`

## Steps
1. **Set the database connection** in `appsettings.json` → `ConnectionStrings:MilkoraDb`
   to a SQL Server the target PCs can reach (e.g. `Server=10.73.9.31;...`).
   The committed value is a safe LocalDB placeholder — it gets **bundled into the
   installer**, so put the real (reachable) server here before building.

2. **Build + copy the Angular UI:**
   ```
   cd ../../Milkora.Client
   ng build --configuration production
   robocopy dist/milkora.client/browser ../src/Milkora.Desktop/wwwroot /MIR
   ```

3. **Build the installer** (from this folder):
   ```
   electronize build /target win
   ```
   Output: `obj/desktop/win/bin/Desktop/Milkora Dairy Farm Tracker Setup 1.0.0.exe`

## Gotchas (already handled in the repo)
- **electronize needs .NET 6**, which isn't installed → run with roll-forward:
  `DOTNET_ROLL_FORWARD=LatestMajor electronize build /target win`
- **Git Bash mangles `/target`** → run with `MSYS_NO_PATHCONV=1` (or use PowerShell).
- **Duplicate `appsettings.json`** (this host references `Milkora.API`) → resolved by
  `Milkora.Desktop.csproj` (`ErrorOnDuplicatePublishOutputFiles=false` + a target that
  drops the API's appsettings from publish, which also keeps the API's secrets out).
- **Icon path**: electron-builder runs from `obj/desktop/win`, so `electron.manifest.json`
  uses an **absolute path** to `Assets/icon.ico`. If you build on a different machine,
  update that absolute path (three places: `win.icon`, `nsis.installerIcon`,
  `nsis.uninstallerIcon`).

## Requirements to RUN the installed app
- The PC must reach the SQL Server in the bundled connection string (same network / VPN).
- First launch may be slower (self-contained runtime unpacks).
- The installer is unsigned → Windows SmartScreen may warn: **More info → Run anyway**.
