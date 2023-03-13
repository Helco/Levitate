using System;
using System.Runtime.InteropServices;

namespace Levitate.Mfc;

// Yes I know this is basically CObArray, don't blame me :(

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal unsafe partial struct CPtrArray
{
    public static readonly CObject.VTable* VirtualTable = (CObject.VTable*)0x004C3098;

    public CObject.VTable* VtPtr;
    public IntPtr* Data;
    public int Size;
    public int Capacity;
    public int GrowBy;

    [Attach(0x00449A45)]
    public partial CPtrArray* Ctor();

    [Attach(0x00449B11)]
    public partial void SetSize(int size, int growBy);
    public void SetSize(int size) => SetSize(size, -1);

    [Attach(0x00449C66)]
    public partial void InsertAt(int atIndex, IntPtr ptr, int repeatCount);
    public void InsertAt(int atIndex, IntPtr ptr) => InsertAt(atIndex, ptr, 1);
}

// no type-safe variant as CObArray would have been used instead of CPtrArray originally
