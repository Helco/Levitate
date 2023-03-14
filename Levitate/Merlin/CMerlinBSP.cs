using System;
using System.Runtime.InteropServices;
using Levitate.Mfc;

namespace Levitate.Merlin;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal unsafe partial struct CMerlinBSP : IRuntimeObject
{
    public static CRuntimeClass* RuntimeClass => (CRuntimeClass*)0x004C5660;
    public static readonly CObject.VTable* VirtualTable = (CObject.VTable*)0x004BD438;

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe record struct Inner(int unk1, float unk2)
    {
        public void Read(CArchive* arc)
        {
            unk1 = arc->ReadInt();
            unk2 = arc->ReadFloat();
        }
    }

    public CMerlinLine @base;
    public short Id, StaticIndex, LeftBsp, RightBsp;
    public int unk_4C;
    public Inner Inner1, Inner2;
    public int unk_60;
    public int unk_64;

    public void Ctor()
    {
        @base.CtorEmpty();
        @base.@base.VtPtr = VirtualTable;
        Id = StaticIndex = LeftBsp = RightBsp = -1;
        Inner1 = new(0, 0);
        Inner2 = new(0, 1.875f);
        unk_4C = unk_60 = unk_64 = 0;
    }

    [Attach(0x41BB80, CallingConvention.StdCall)]
    public static CMerlinBSP* Create(CMerlinBSP* bsp)
    {
        if (bsp != null)
            bsp->Ctor();
        return bsp;
    }

    [Attach(0x41BC40)]
    public void Serialize(CArchive* arc)
    {
        @base.Serialize(arc);
        if (!arc->Mode.HasFlag(ArchiveMode.Load))
            throw new NotImplementedException();
        Id = arc->ReadShort();
        StaticIndex = arc->ReadShort();
        LeftBsp = arc->ReadShort();
        RightBsp = arc->ReadShort();
        unk_60 = arc->ReadUShort();

        Inner1.Read(arc);
        Inner2.Read(arc);

        arc->Skip(arc->ReadUShort());
    }
}
