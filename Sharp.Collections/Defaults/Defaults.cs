using System;

namespace Sharp.Collections
{
    public static class Defaults
    {
        public static int SegmentSize { get; }

        static Defaults()
        {
            SegmentSize = Settings.DangerousTryGet(out DefaultSettings? settings)
                ? settings!.SegmentSize
                : IntPtr.Size * Constants.ByteSizeInBits;
        }
    }
}
