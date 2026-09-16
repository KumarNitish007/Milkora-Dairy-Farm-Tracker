namespace Milkora.SyncClient;

/// <summary>Configuration for the device-side sync engine.</summary>
public sealed class MilkoraSyncSettings
{
    /// <summary>Path to the local SQLite database file (created if missing).</summary>
    public string LocalDbPath { get; set; } = "dairy_local.db";

    /// <summary>Full URL of the API sync endpoint, e.g. https://localhost:7150/api/sync.</summary>
    public string ServerSyncUrl { get; set; } = "https://localhost:7150/api/sync";

    /// <summary>Automatic sync interval when online. 0 disables the timer.</summary>
    public int SyncIntervalMinutes { get; set; } = 10;

    /// <summary>Dev only: trust a self-signed localhost certificate.</summary>
    public bool AcceptAnyServerCertificate { get; set; }

    /// <summary>Retry attempts for a failed sync before reporting Error.</summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>Delay between retries.</summary>
    public int RetryDelaySeconds { get; set; } = 5;
}
