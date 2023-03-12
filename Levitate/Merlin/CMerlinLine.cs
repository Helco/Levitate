using System;
using System.Runtime.InteropServices;
using Levitate.Mfc;

namespace Levitate.Merlin;

[Flags]
internal enum LineFlags : uint
{
    DegradedToPoint = 1,
    DegradedWidth = 2,
    DegradedHeight = 4
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal unsafe partial struct CMerlinLine
{
    public static readonly CMerlinObject.VTable* VirtualTable = (CMerlinObject.VTable*)0x4BC110;

    public CMerlinObject @base;
    public IVector2 P0, P1;
    public LineFlags Flags;
    public int MinX, MaxX, MinY, MaxY;
    public IVector2 Dir;
    public int LengthSqr;

    [Attach(0x0040F760)]
    public CMerlinLine* Ctor(int x0, int y0, int x1, int y1)
    {
        @base.Ctor();
        @base.VtPtr = VirtualTable;
        P0 = new(x0, y0);
        P1 = new(x1, y1);
        CalcDerivatives();
        fixed (CMerlinLine* pThis = &this) return pThis;
    }

    [Attach(0x004010C0)]
    public void CalcDerivatives()
    {
        MinX = Math.Min(P0.X, P1.X);
        MaxX = Math.Max(P0.X, P1.X);
        MinY = Math.Min(P0.Y, P1.Y);
        MaxY = Math.Max(P0.Y, P1.Y);
        Dir = P1 - P0;
        LengthSqr = Dir.LengthSqr;
        Flags =
            (P0 == P1 ? LineFlags.DegradedToPoint : 0) |
            (Dir.X == 0 ? LineFlags.DegradedWidth : 0) |
            (Dir.Y == 0 ? LineFlags.DegradedHeight : 0);
    }

    [Attach(0x00414740)]
    public void Serialize(CArchive* arc)
    {
        @base.Serialize(arc);
        if (arc->Mode.HasFlag(ArchiveMode.Load))
        {
            P0 = new(arc->ReadShort(), arc->ReadShort());
            P1 = new(arc->ReadShort(), arc->ReadShort());
            arc->Skip(arc->ReadUShort());
            CalcDerivatives();
        }
        else
            throw new NotImplementedException();
    }
}
