using System.Runtime.InteropServices;

namespace xNotepad64
{
    public readonly record struct SystemMemorySnapshot(ulong TotalPhysicalBytes, ulong AvailablePhysicalBytes);

    public static class SystemMemoryInfo
    {
        public static bool TryGetSnapshot(out SystemMemorySnapshot snapshot)
        {
            var memoryStatus = new MemoryStatusEx();
            if (!GlobalMemoryStatusEx(memoryStatus))
            {
                snapshot = default;
                return false;
            }

            snapshot = new SystemMemorySnapshot(memoryStatus.ullTotalPhys, memoryStatus.ullAvailPhys);
            return true;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GlobalMemoryStatusEx([In, Out] MemoryStatusEx lpBuffer);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private sealed class MemoryStatusEx
        {
            public uint dwLength = (uint)Marshal.SizeOf<MemoryStatusEx>();
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }
    }
}
