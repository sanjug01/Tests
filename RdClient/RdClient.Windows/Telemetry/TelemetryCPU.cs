namespace RdClient.Telemetry
{
    using System.Runtime.InteropServices;
    using System;

    public enum Platform
    {
        X86,
        X64,
        IA64,
        Unknown
    }

    class ClientCPU
    {
        const ushort PROCESSOR_ARCHITECTURE_INTEL = 0;
        const ushort PROCESSOR_ARCHITECTURE_IA64 = 6;
        const ushort PROCESSOR_ARCHITECTURE_AMD64 = 9;
        const ushort PROCESSOR_ARCHITECTURE_UNKNOWN = 0xFFFF;

        [StructLayout(LayoutKind.Sequential)]
        internal struct SYSTEM_INFO
        {
            public ushort wProcessorArchitecture;
            public ushort wReserved;
            public uint dwPageSize;
            public IntPtr lpMinimumApplicationAddress;
            public IntPtr lpMaximumApplicationAddress;
            public UIntPtr dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public ushort wProcessorLevel;
            public ushort wProcessorRevision;
        };

        [DllImport("kernel32.dll")]
        internal static extern void GetNativeSystemInfo(ref SYSTEM_INFO lpSystemInfo);
        static public string GetPlatform()
        {
            string platformValue = string.Empty;
            SYSTEM_INFO sysInfo = new SYSTEM_INFO();
            GetNativeSystemInfo(ref sysInfo);
            switch(sysInfo.wProcessorArchitecture)
            {
                case PROCESSOR_ARCHITECTURE_INTEL:
                    platformValue = PROCESSOR_ARCHITECTURE_INTEL.ToString();
                    break;

                case PROCESSOR_ARCHITECTURE_IA64:
                    platformValue = PROCESSOR_ARCHITECTURE_IA64.ToString();
                    break;

                case PROCESSOR_ARCHITECTURE_AMD64:
                    platformValue = PROCESSOR_ARCHITECTURE_AMD64.ToString();
                    break;

                default:
                    platformValue = PROCESSOR_ARCHITECTURE_UNKNOWN.ToString();
                    break;
            }

            return platformValue;
        }
    }

}