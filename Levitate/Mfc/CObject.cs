using System;
using System.Runtime.InteropServices;

namespace Levitate.Mfc;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal unsafe partial struct CObject
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct VTable
    {
        public delegate* unmanaged[Thiscall]<CObject*, CRuntimeClass*> GetRuntimeClass;
        public delegate* unmanaged[Thiscall]<CObject*, void> Dtor;
        public delegate* unmanaged[Thiscall]<CObject*, CArchive*, void> Serialize;
    }

    public VTable* VtPtr;

    [Attach(0x44E710)]
    public partial bool IsKindOf(CRuntimeClass* clazz);
}
