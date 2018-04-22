namespace AiDollar.Infrastructure.Configuration
{
    public interface ISettingsBase
    {
        bool ChaosMonkey { get; }
        bool ShowTelemetry { get; }
        string LogPath { get; }
        string LogFilename { get; }
        string LogArchivePath { get; }
        int MaximumMessageSize { get; }
        TelemetryKind TelemetryKind { get; }
    }
}
