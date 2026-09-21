// Fungsi file: Mengatur interval heartbeat IDN, timeout sesi, dan pemeriksaan sesi kedaluwarsa.
namespace Cashflowpoly.Api.Infrastructure;

public sealed class SessionLifecycleOptions
{
    public bool Enabled { get; set; } = true;
    public int HeartbeatIntervalSeconds { get; set; } = 1800;
    public int StartedTimeoutSeconds { get; set; } = 3600;
    public int CreatedTimeoutSeconds { get; set; } = 3600;
    public int SweepIntervalSeconds { get; set; } = 15;
}
