const sharp = require('sharp');
const fs = require('fs');
const path = require('path');

const BASE = "C:/NK_BACK/OneDrive - Shapoorji Pallonji & Company Pvt. Limited/NITISH/MILKORA/Milkora.DairyFarm";
const B = path.join(BASE, 'branding');
const square = fs.readFileSync(path.join(B, 'milkora-icon-square.svg'));
const rounded = fs.readFileSync(path.join(B, 'milkora-icon-rounded.svg'));
const fg = fs.readFileSync(path.join(B, 'milkora-foreground.svg'));

const mk = d => fs.mkdirSync(d, { recursive: true });
async function png(svg, size, out) {
  mk(path.dirname(out));
  await sharp(svg, { density: 384 }).resize(size, size, { fit: 'contain', background: { r: 0, g: 0, b: 0, alpha: 0 } }).png().toFile(out);
  console.log('  ' + path.relative(BASE, out) + '  (' + size + ')');
}

(async () => {
  // 1) PWA icons (overwrite the placeholders ng add created)
  console.log('PWA icons:');
  for (const s of [72, 96, 128, 144, 152, 192, 384, 512])
    await png(square, s, path.join(BASE, 'Milkora.Client/public/icons/icon-' + s + 'x' + s + '.png'));

  // 2) Favicon source PNGs (rounded) -> assembled into .ico by Python
  console.log('Favicon PNGs:');
  const favDir = path.join(BASE, 'branding/_fav');
  for (const s of [16, 32, 48, 64, 256]) await png(rounded, s, path.join(favDir, 'fav-' + s + '.png'));
  // also a crisp SVG favicon copy for modern browsers
  fs.copyFileSync(path.join(B, 'milkora-icon-rounded.svg'), path.join(BASE, 'Milkora.Client/public/favicon.svg'));
  console.log('  Milkora.Client/public/favicon.svg');

  // 3) Desktop (Electron) icon PNGs (rounded) -> .ico by Python
  console.log('Desktop icon PNGs:');
  const desktopAssets = path.join(BASE, 'src/Milkora.Desktop/Assets');
  for (const s of [16, 24, 32, 48, 64, 128, 256]) await png(rounded, s, path.join(desktopAssets, '_ico-' + s + '.png'));
  await png(rounded, 512, path.join(desktopAssets, 'icon.png'));

  // 4) Android launcher icons (Capacitor res/) — legacy square + round + adaptive foreground
  console.log('Android mipmaps:');
  const dens = { mdpi: 48, hdpi: 72, xhdpi: 96, xxhdpi: 144, xxxhdpi: 192 };
  const fgDens = { mdpi: 108, hdpi: 162, xhdpi: 216, xxhdpi: 324, xxxhdpi: 432 };
  const res = path.join(BASE, 'resources/android');
  for (const [d, s] of Object.entries(dens)) {
    await png(square, s, path.join(res, 'mipmap-' + d, 'ic_launcher.png'));
    await png(square, s, path.join(res, 'mipmap-' + d, 'ic_launcher_round.png'));
  }
  for (const [d, s] of Object.entries(fgDens))
    await png(fg, s, path.join(res, 'mipmap-' + d, 'ic_launcher_foreground.png'));

  // Play Store hi-res icon
  await png(square, 512, path.join(res, 'play-store-icon.png'));

  console.log('DONE');
})();
