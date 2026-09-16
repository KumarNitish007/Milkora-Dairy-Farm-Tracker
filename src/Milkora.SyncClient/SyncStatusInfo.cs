namespace Milkora.SyncClient;

/// <summary>Mirrors the states shown in the UI sync indicator.</summary>
public enum SyncState
{
    Idle,
    Syncing,
    Synced,
    Pending,   // local changes waiting (offline)
    Offline,
    Error,
}

/// <summary>Immutable snapshot of the current sync status, raised on every change.</summary>
public sealed record SyncStatusInfo
{
    public SyncState State { get; init; } = SyncState.Idle;
    public string? Message { get; init; }
    public DateTime? LastSyncedAt { get; init; }
    public long ChangesUploaded { get; init; }
    public long ChangesDownloaded { get; init; }
    public int TotalConflicts { get; init; }
    public string? Error { get; init; }

    public string Label => State switch
    {
        SyncState.Synced => "Synced",
        SyncState.Syncing => "Syncing…",
        SyncState.Pending => "Pending",
        SyncState.Offline => "Offline",
        SyncState.Error => "Sync error",
        _ => "Idle",
    };
}
