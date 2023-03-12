using System;
using System.Runtime.InteropServices;

namespace Levitate;

internal unsafe partial struct Globals
{
    [Attach(0x0043E2CF, CallingConvention.Cdecl)]
    public static partial byte* New(int size);

    [Attach(0x0043E34A, CallingConvention.Cdecl)]
    public static partial void Delete(byte* mem);
}
