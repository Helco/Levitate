using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Levitate.Mfc;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal unsafe partial struct CGdiObject : IRuntimeObject
{
    public static readonly CObject.VTable* VirtualTable = (CObject.VTable*)0x004BB6B8;

    public CObject @base;
    public void* Handle;

    public static CRuntimeClass* RuntimeClass => throw new NotImplementedException("Did not implement RuntimeClass for CGdiObject");

    public void Ctor()
    {
        @base.VtPtr = VirtualTable;
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal unsafe partial struct CBitmap : IRuntimeObject
{
    public static readonly CObject.VTable* VirtualTable = (CObject.VTable*)0x004BBCE0;

    public CGdiObject @base;

    public static CRuntimeClass* RuntimeClass => throw new NotImplementedException("Did not implement RuntimeClass for CBitmap");

    public void Ctor()
    {
        @base.@base.VtPtr = VirtualTable;
    }
}
