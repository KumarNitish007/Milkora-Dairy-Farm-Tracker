using Dotmim.Sync;
using Dotmim.Sync.Enumerations;
using Dotmim.Sync.SqlServer;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Milkora.Application;
using Milkora.Desktop.Services;
using Milkora.Infrastructure;
using Milkora.SyncClient;

var builder = WebApplication.CreateBuilder(args);

// Electron.NET: lets `electronize` launch this host inside a Chromium window.
builder.WebHost.UseElectron(args);

// ---- Controllers: reuse the standalone API's controllers via ApplicationPart --
builder.Services
    .AddControllers()
    .AddApplicationPart(typeof(Milkora.API.Controllers.AnimalsController).Assembly);

builder.Services.AddEndpointsApiExplorer();

// ---- Layers (same DI as the API) -------------------------------------------
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

// ---- Sync server endpoint (local SQLite clients <-> central MS SQL) ---------
var connectionString = builder.Configuration.GetConnectionString("MilkoraDb")!;
builder.Services.AddSyncServer(
    new SqlSyncProvider(connectionString),
    Milkora.Sync.SyncTables.CreateSetup(),
    new SyncOptions { ConflictResolutionPolicy = ConflictResolutionPolicy.ServerWins });
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(o =>
{
    o.IdleTimeout = TimeSpan.FromMinutes(30);
    o.Cookie.HttpOnly = true;
    o.Cookie.IsEssential = true;
});

// ---- Device sync engine (background) ---------------------------------------
var syncSettings = new MilkoraSyncSettings();
builder.Configuration.GetSection("Sync").Bind(syncSettings);
builder.Services.AddSingleton(syncSettings);
builder.Services.AddSingleton<DesktopSyncCoordinator>();
builder.Services.AddHostedService<SyncHostedService>();

var app = builder.Build();

// Angular SPA (served from wwwroot) + API.
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseSession();          // required by the Dotmim.Sync web server
app.MapControllers();
app.MapFallbackToFile("index.html");   // deep links resolve to the SPA

// Open the desktop window once Kestrel is up (only under Electron).
if (HybridSupport.IsElectronActive)
{
    app.Lifetime.ApplicationStarted.Register(() => _ = Task.Run(async () =>
    {
        var window = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
        {
            Width = 1280,
            Height = 820,
            Show = false,
            Title = "Milkora Dairy Farm Tracker",
            AutoHideMenuBar = true,
        });
        window.OnReadyToShow += () => window.Show();
    }));
}

app.Run();
