using System;
using System.Runtime.InteropServices;

namespace Levitate.Utils;

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 4)]
internal readonly struct Bool32
{
    [FieldOffset(0)] private readonly int data;

    private Bool32(bool b) => data = b ? 1 : 0;

    public static implicit operator bool(Bool32 val) => val.data != 0;
    public static implicit operator Bool32(bool val) => new(val);
    public static bool operator true(Bool32 val) => val.data != 0;
    public static bool operator false(Bool32 val) => val.data == 0;
    public static Bool32 operator !(Bool32 val) => new(val.data == 0);
}
