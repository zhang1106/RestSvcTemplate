namespace AiDollar.Infrastructure.Configuration
{
    public class RingBufferSettings : AppSettingsBase
    {
        public RingBufferSettings(string prefix) : base(prefix)
        {
            
        }

        public int RingSize => GetValue(nameof(RingSize), Prefix, GetGlobalValue(() => RingSize, 524288));
        public int BufferSize => GetValue(nameof(BufferSize), Prefix, GetGlobalValue(() => BufferSize, 1024));
    }
}
