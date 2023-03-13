using System;
using System.Runtime.InteropServices;

namespace Levitate.Mfc;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal unsafe partial struct CObArray
{
    public IntPtr VtPtr;
    public CObject** Data;
    public int Size, Capacity, GrowBy;

    [Attach(0x004494CC)]
    public partial CObArray* Ctor();
}

#pragma warning disable CS9084 // Struct member returns 'this' or other instance members by reference
[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal unsafe struct CObArray<T> where T : unmanaged
{
    public CObArray Raw;
    public T** Data => (T**)Raw.Data;
    public ref int Size => ref Raw.Size;
    public ref int Capacity => ref Raw.Capacity;

    public void Ctor() => Raw.Ctor();
}
#pragma warning restore CS9084 // Struct member returns 'this' or other instance members by reference
