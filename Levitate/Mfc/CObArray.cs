using System;
using System.Runtime.InteropServices;

namespace Levitate.Mfc;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal unsafe partial struct CObArray
{
    public IntPtr VtPtr;
    public CObject** Data;
    public int Size, Capacity, GrowBy;
}
