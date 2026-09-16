# Milkora branding assets

Master mark: a white cow face on the brand green `#2E7D32` (accents `#1B5E20`, `#A5D6A7`, `#C8E6C9`).

## Source SVGs (edit these, then regenerate)
| File | Use |
|------|-----|
| `milkora-icon-square.svg` | full-bleed app icon (PWA, Android, maskable) |
| `milkora-icon-rounded.svg` | rounded app icon (favicon, Windows desktop) |
| `milkora-foreground.svg` | Android adaptive-icon foreground (transparent, safe-zone) |
| `milkora-wordmark.svg` | horizontal logo (icon + "Milkora / Dairy Farm Tracker") |

## Generated & wired up
- **Web / PWA** — `Milkora.Client/public/icons/icon-*.png` (72→512) + `public/favicon.ico` + `public/favicon.svg`; referenced from `index.html` and `manifest.webmanifest`. The nav-bar brand uses `favicon.svg`.
- **Desktop (Electron)** — `src/Milkora.Desktop/Assets/icon.ico` (+ `icon.png`); referenced from `electron.manifest.json` (`win.icon`, `nsis.installerIcon`).
- **Android / APK** — `resources/android/`:
  - `mipmap-<density>/ic_launcher.png`, `ic_launcher_round.png`, `ic_launcher_foreground.png`
  - `mipmap-anydpi-v26/ic_launcher.xml`, `ic_launcher_round.xml` (adaptive icon)
  - `values/ic_launcher_background.xml` (background color `#2E7D32`)
  - `play-store-icon.png` (512, for the Play Console listing)

## Applying the Android icons (after `npx cap add android`)
Copy `resources/android/*` into `android/app/src/main/res/`, overwriting the generated
`mipmap-*` folders and merging `values/`. Then build the APK. (Or use `@capacitor/assets`
with `milkora-icon-square.svg` as the source to regenerate automatically.)

## Regenerate all raster sizes
Requires Node. From a scratch folder: `npm i sharp`, then run the `generate.js` used to
build these (rasterizes each source SVG to every size; `.ico` files are assembled from the
rounded PNGs with Pillow). Re-run after editing any source SVG.
