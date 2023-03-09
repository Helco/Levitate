using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Levitate
{
    public static class Detours
    {
        // This also works with CharSet.Ansi as long as the calling function uses the same character set.
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct STARTUPINFO
        {
            public Int32 cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public Int32 dwX;
            public Int32 dwY;
            public Int32 dwXSize;
            public Int32 dwYSize;
            public Int32 dwXCountChars;
            public Int32 dwYCountChars;
            public Int32 dwFillAttribute;
            public Int32 dwFlags;
            public Int16 wShowWindow;
            public Int16 cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        [DllImport("Detours", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode)]
        public static extern bool DetourCreateProcessWithDllW(
            string? applicationName,
            string? commandLine,
            IntPtr processAttributes,
            IntPtr threadAttributes,
            bool inheritHandles,
            uint creationFlags,
            IntPtr environment,
            string? currentDirectory,
            in STARTUPINFO startupInfo,
            out PROCESS_INFORMATION processInformation,
            [MarshalAs(UnmanagedType.LPStr)] string dllName,
            IntPtr createProcessW);

        [DllImport("Detours", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode)]
        public static extern int DetourCreateProcessWithDllsW(
            string? applicationName,
            string? commandLine,
            IntPtr processAttributes,
            IntPtr threadAttributes,
            bool inheritHandles,
            uint creationFlags,
            IntPtr environment,
            string? currentDirectory,
            in STARTUPINFO startupInfo,
            out PROCESS_INFORMATION processInformation,
            int dllCount,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr)] string[] dlls,
            IntPtr createProcessW);
    }
}
