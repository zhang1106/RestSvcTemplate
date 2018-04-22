using System;

namespace AiDollar.Infrastructure.Configuration
{
    public abstract class SettingsBase : AppSettingsBase, ISettingsBase
    {
        protected SettingsBase(string prefix = "") : base(prefix)
        {
            Journal = new JournalSettings(prefix + "Journal.");
        }

        public JournalSettings Journal { get; }

        public string LogPath
        {
            get { return GetValue(() => LogPath, GetGlobalValue(() => LogPath)); }
        }

        public string LogArchivePath
        {
            get { return GetValue(() => LogArchivePath, GetGlobalValue(() => LogArchivePath)); }
        }

        public string LogFilename => GetType().Name.Replace("Settings", string.Empty) + ".log";

        public TelemetryKind TelemetryKind
        {
            get { return GetValue(() => TelemetryKind, GetGlobalValue(() => TelemetryKind, TelemetryKind.LogFile)); }
        }

        public int MaximumMessageSize
        {
            get { return GetValue(() => MaximumMessageSize, GetGlobalValue(() => MaximumMessageSize, 1024*1024*25)); }
        }

        public bool ChaosMonkey
        {
            get { return GetValue(() => ChaosMonkey, GetGlobalValue(() => ChaosMonkey)); }
        }

        public bool ShowTelemetry
        {
            get { return GetValue(() => ShowTelemetry, GetGlobalValue(() => ShowTelemetry)); }
        }

        public TimeSpan RecoveryTimeout
        {
            get
            {
                return GetGlobalValue(() => RecoveryTimeout,
                    GetGlobalValue(() => RecoveryTimeout, TimeSpan.FromSeconds(10)));
            }
        }

        public TimeSpan HeartbeatInterval
        {
            get
            {
                return GetValue(() => HeartbeatInterval,
                    GetGlobalValue(() => HeartbeatInterval, TimeSpan.FromSeconds(30)));
            }
        }

        public TimeSpan TelemetryPublishInterval
        {
            get
            {
                if (!ShowTelemetry)
                {
                    return TimeSpan.Zero;
                }

                return GetValue(() => TelemetryPublishInterval,
                    GetGlobalValue(() => TelemetryPublishInterval, TimeSpan.FromSeconds(10)));
            }
        }

        public bool LogAllMessages
        {
            get { return GetValue(() => LogAllMessages, GetGlobalValue(() => LogAllMessages)); }
        }
    }
}
