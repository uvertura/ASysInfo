using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LibASysInfo
{
    public static class MemoryInfo
    {
        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPerformanceInfo([Out] out MemoryInformation memoryInformation, [In] int Size);

        [StructLayout(LayoutKind.Sequential)]
        public struct MemoryInformation
        {
            public int Size;
            public IntPtr CommitTotal;
            public IntPtr CommitLimit;
            public IntPtr CommitPeak;
            public IntPtr PhysicalTotal;
            public IntPtr PhysicalAvailable;
            public IntPtr SystemCache;
            public IntPtr KernelTotal;
            public IntPtr KernelPaged;
            public IntPtr KernelNonPaged;
            public IntPtr PageSize;
            public int HandlesCount;
            public int ProcessCount;
            public int ThreadCount;
        }

        public static long GetPhysicalAvailableMemoryInMiB()
        {
            var pi = new MemoryInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToInt64((pi.PhysicalAvailable.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            }
            return -1;
        }

        public static long GetTotalMemory()
        {
            var pi = new MemoryInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToInt64(pi.PhysicalTotal.ToInt64() * pi.PageSize.ToInt64());
            }
            return -1;
        }

        public static byte GetAvailablePercentMemory()
        {
            var pi = new MemoryInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                var totalMiB = Convert.ToInt64(pi.PhysicalTotal.ToInt64()*pi.PageSize.ToInt64());
                var availableMiB = Convert.ToInt64(pi.PhysicalAvailable.ToInt64()*pi.PageSize.ToInt64());
                return (byte) (availableMiB/(totalMiB/100));
            }
            return 0;
        }

        public static byte GetUsagePercentMemory()
        {
            var available = GetAvailablePercentMemory();
            return (byte)(0 == available ? 0 : 100 - available);
        }
    }
    
}
