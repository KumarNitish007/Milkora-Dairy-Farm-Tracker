using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Milkora.SyncClient;

// Modes:
//   (none)  -> start background sync (initial + timer + reconnect), run until Ctrl+C
//   once    -> a single sync, then exit
//   demo    -> download, write a row LOCALLY (offline-first), sync up, verify, exit
var mode = args.Length > 0 ? args[0].ToLowerInvariant() : "run";

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true)
    .Build();

var settings = new MilkoraSyncSettings();
config.GetSection("Sync").Bind(settings);

using var loggerFactory = LoggerFactory.Create(b => b.AddSimpleConsole(o => { o.SingleLine = true; o.TimestampFormat = "HH:mm:ss "; }).SetMinimumLevel(LogLevel.Information));

await using var manager = new MilkoraSyncManager(settings, loggerFactory.CreateLogger<MilkoraSyncManager>());
manager.StatusChanged += (_, s) =>
    Console.WriteLine($"  [status] {s.Label,-10} up={s.ChangesUploaded} down={s.ChangesDownloaded} conflicts={s.TotalConflicts}{(s.Error is null ? "" : "  err=" + s.Error)}");

Console.WriteLine($"Milkora sync client — mode='{mode}'  db='{settings.LocalDbPath}'  server='{settings.ServerSyncUrl}'\n");

switch (mode)
{
    case "once":
        await manager.SynchronizeAsync();
        break;

    case "demo":
        Console.WriteLine("1) Initial sync (download from server)…");
        await manager.SynchronizeAsync();
        PrintCounts(settings.LocalDbPath);

        Console.WriteLine("\n2) Writing a new animal LOCALLY (offline-first capture)…");
        var tag = "OFFLINE-" + DateTime.Now.ToString("HHmmss");
        InsertLocalAnimal(settings.LocalDbPath, tag);
        Console.WriteLine($"   inserted '{tag}' into local SQLite.");

        Console.WriteLine("\n3) Sync (upload local change to server)…");
        var result = await manager.SynchronizeAsync();
        Console.WriteLine($"   uploaded={result.ChangesUploaded}");
        Console.WriteLine(result.ChangesUploaded > 0
            ? "   ✅ local offline write reached the server."
            : "   ⚠️ nothing uploaded (see notes).");
        break;

    default:
        using (var cts = new CancellationTokenSource())
        {
            Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };
            await manager.StartAsync(cts.Token);
            Console.WriteLine($"\nBackground sync running (every {settings.SyncIntervalMinutes} min). Press Ctrl+C to stop.");
            try { await Task.Delay(Timeout.Infinite, cts.Token); } catch (OperationCanceledException) { }
        }
        break;
}

Console.WriteLine("\nDone.");

// ---- local SQLite helpers ---------------------------------------------------

static void PrintCounts(string dbPath)
{
    using var conn = new SqliteConnection($"Data Source={dbPath}");
    conn.Open();
    Console.WriteLine("   local row counts:");
    foreach (var t in new[] { "Animals", "MilkProductionLog", "MilkSales", "Expenses", "Income", "HealthRecords", "BreedingRecords", "FeedInventory" })
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = $"SELECT COUNT(*) FROM \"{t}\"";
        try { Console.WriteLine($"     {t}: {cmd.ExecuteScalar()}"); }
        catch (Exception ex) { Console.WriteLine($"     {t}: ERROR {ex.Message}"); }
    }
}

// Insert an Animal into the local SQLite DB using a client-generated GUID, matching
// however Dotmim.Sync stored existing GUIDs (TEXT vs BLOB) so the upload maps cleanly.
static void InsertLocalAnimal(string dbPath, string tag)
{
    using var conn = new SqliteConnection($"Data Source={dbPath}");
    conn.Open();

    var guidIsText = true;
    using (var probe = conn.CreateCommand())
    {
        probe.CommandText = "SELECT typeof(AnimalId) FROM Animals LIMIT 1";
        var t = probe.ExecuteScalar() as string;
        if (string.Equals(t, "blob", StringComparison.OrdinalIgnoreCase)) guidIsText = false;
    }

    var id = Guid.NewGuid();
    var nowIso = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");

    using var cmd = conn.CreateCommand();
    cmd.CommandText = @"INSERT INTO ""Animals"" (AnimalId, TagNumber, Name, Type, Status, CreatedAt, UpdatedAt)
                        VALUES ($id, $tag, $name, $type, $status, $c, $u)";
    cmd.Parameters.AddWithValue("$id", guidIsText ? id.ToString() : (object)id.ToByteArray());
    cmd.Parameters.AddWithValue("$tag", tag);
    cmd.Parameters.AddWithValue("$name", "Offline Cow");
    cmd.Parameters.AddWithValue("$type", "Cow");
    cmd.Parameters.AddWithValue("$status", "Milking");
    cmd.Parameters.AddWithValue("$c", nowIso);
    cmd.Parameters.AddWithValue("$u", nowIso);
    cmd.ExecuteNonQuery();
}
