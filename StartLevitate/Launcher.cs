using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static Levitate.Detours;

namespace Levitate
{
    internal class Launcher
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint ResumeThread(IntPtr hThread);

        private static void Main(string[] args)
        {
            STARTUPINFO startupInfo = new()
            {
                cb = Marshal.SizeOf<STARTUPINFO>()
            };

            if (!DetourCreateProcessWithDllW(
                @"C:\dev\Levitate\HoverGame\Hover.exe",
                "",
                0,
                0,
                false,
                0, // create suspended
                0,
                @"C:\dev\Levitate\HoverGame",
                startupInfo,
                out var processInfo,
                Path.Combine(Path.GetDirectoryName(Environment.ProcessPath)!, "LevitateLoader.dll"),
                0))
                throw new Exception("Could not start hover");

            //ResumeThread(processInfo.hThread);

            Console.ReadKey(true);
        }
    }
}