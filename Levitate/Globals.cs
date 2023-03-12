using System;
using System.Runtime.InteropServices;

namespace Levitate;

internal unsafe partial struct Globals
{
    private static readonly nint** progressCallback = (nint**)0x004C4DAC;
    public static delegate* unmanaged[Cdecl]<int, int, void> ProgressCallback =>
        (delegate* unmanaged[Cdecl]<int, int, void>)*progressCallback;

    public static readonly int* ProgressNum = (int*)0x004C4DA8;

    [Attach(0x0043E2CF, CallingConvention.Cdecl)]
    public static partial byte* New(int size);

    [Attach(0x0043E34A, CallingConvention.Cdecl)]
    public static partial void Delete(byte* mem);
}
