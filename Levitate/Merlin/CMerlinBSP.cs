using System;
using System.Runtime.InteropServices;
using Levitate.Mfc;

namespace Levitate.Merlin;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal unsafe partial struct CMerlinBSP
{
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
    public short id, staticIndex, leftBsp, rightBsp;
    public int unk_4C;
    public Inner inner1, inner2;
    public int unk_60;
    public int unk_64;

    public void Ctor()
    {
        @base.CtorEmpty();
        @base.@base.VtPtr = VirtualTable;
        id = staticIndex = leftBsp = rightBsp = -1;
        inner1 = new(0, 0);
        inner2 = new(0, 1.875f);
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
        id = arc->ReadShort();
        staticIndex = arc->ReadShort();
        leftBsp = arc->ReadShort();
        rightBsp = arc->ReadShort();
        unk_60 = arc->ReadUShort();

        inner1.Read(arc);
        inner2.Read(arc);

        arc->Skip(arc->ReadUShort());
    }
}
