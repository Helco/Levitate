using System;
using System.Runtime.InteropServices;
using Levitate.Mfc;

namespace Levitate.Merlin;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal unsafe partial struct CMerlinLocation
{
    public static readonly CMerlinObject.VTable* VirtualTable = (CMerlinObject.VTable*)0x004BD3C0;
    public static readonly CMapStringToOb* ByName = (CMapStringToOb*)0x004A4768;

    public CMerlinObject @base;
    public short X, Y;
    public short Rotation;
    public short Z;

    public CMerlinLocation* Ctor()
    {
        @base.Ctor();
        @base.VtPtr = VirtualTable;
        X = Y = Rotation = Z = 0;
        fixed (CMerlinLocation* pThis = &this) return pThis;
    }

    [Attach(0x415070, CallingConvention.StdCall)]
    public static CMerlinLocation* Create(CMerlinLocation* loc)
    {
        if (loc != null)
            loc->Ctor();
        return loc;
    }

    [Attach(0x41B940)]
    public void Serialize(CArchive* arc)
    {
        @base.Serialize(arc);
        if (arc->Mode.HasFlag(ArchiveMode.Load))
        {
            if (@base.Name.Length > 0)
                fixed (CMerlinLocation* pThis = &this)
                    *ByName->Lookup(@base.Name.Data) = (CObject*)pThis;
            X = arc->ReadShort();
            Y = arc->ReadShort();
            Rotation = arc->ReadShort();
            Z = arc->ReadShort();
            arc->Skip(arc->ReadUShort());
        }
        else
            throw new NotImplementedException();
    }
}
