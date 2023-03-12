using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static Levitate.Detours;

namespace Levitate;

internal class Launcher
{
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

        var process = Process.GetProcessById(processInfo.dwProcessId);
        process.WaitForExit();
    }
}