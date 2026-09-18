# Running Milkora on mobile

A phone can't run the .NET backend, so **the API must be hosted somewhere the phone can
reach** (and that server must reach the SQL database). The standalone API now also serves
the Angular app (single origin), so hosting the API hosts the whole thing.

There are two ways to get Milkora onto a phone.

---

## Option A — Install as a PWA (recommended, no Android tools)

1. **Host the app over HTTPS.** Publish `Milkora.API` to a server (IIS, a VM, etc.) with
   HTTPS. Before publishing, copy the web build into the API's `wwwroot`:
   ```
   cd Milkora.Client
   ng build --configuration production
   robocopy dist/milkora.client/browser ../src/Milkora.API/wwwroot /MIR
   ```
   Then publish the API and set its `ConnectionStrings:MilkoraDb` to your SQL Server.
2. On the phone, open the HTTPS URL in **Chrome (Android)** or **Safari (iOS)**.
3. Menu → **Install app** (Android) / Share → **Add to Home Screen** (iOS).

You get the cow icon on the home screen, full-screen, with offline caching. No app store.

---

## Option B — Native Android APK (Capacitor)

The Capacitor project is already set up in `android/` with the cow launcher icons.
You only need **Android Studio + JDK 17** installed to build the APK.

1. **Point the app at your hosted API.** Edit `src/environments/environment.mobile.ts`
   and set `apiBaseUrl` to your server's absolute URL, e.g. `https://milkora.mycompany.com/api`.
2. **Build the web app (mobile config) and sync it:**
   ```
   ng build --configuration mobile
   npx cap copy android
   ```
3. **Build the APK** (either):
   - Android Studio: `npx cap open android` → Build ▸ Build APK(s) / Generate Signed Bundle.
   - CLI: `cd android && ./gradlew assembleDebug`
     → output: `android/app/build/outputs/apk/debug/app-debug.apk`
4. Copy the `.apk` to the phone and install it (enable "Install unknown apps").
   The launcher icon is the Milkora cow.

> iOS: `npx cap add ios` then build in Xcode on a Mac.

---

## App identity (already configured)
- App id: `com.milkora.dairyfarmtracker`
- App name: `Milkora`
- Adaptive launcher icon: white cow on green `#2E7D32` (`android/app/src/main/res/mipmap-*`)
