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
    public static T* New<T>() where T : unmanaged => (T*)New(sizeof(T));

    [Attach(0x0043E34A, CallingConvention.Cdecl)]
    public static partial void Delete(byte* mem);

    [Attach(0x00448A3C, CallingConvention.Cdecl)]
    public static partial void ThrowNoMfcObjectAlloc();

    [Attach(0x00448A5A, CallingConvention.Cdecl)]
    public static partial void ThrowNoMfcSerialization();

    [Attach(0x004514FA, CallingConvention.StdCall)]
    public static partial void AfxThrowArchiveException(int code);
}
