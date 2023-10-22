using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Levitate.Merlin;
using Levitate.Mfc;
using Levitate.Utils;

namespace Levitate.Game;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal unsafe partial struct CWallRenderPoint
{
    public int Index;
    public Bool32 IsLeft, IsDynamic;
    public CMerlinLine* Line;
    public CMerlinBSP* BSP;
    public CWallRenderPoint** OverwritesPoint;

    static CWallRenderPoint()
    {
        Debug.Assert(sizeof(CWallRenderPoint) == 0x18);
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal unsafe partial struct CTracedPoint
{
    public int f0, f4, f8;
    public Bool32 IsDynamic;
    public CMerlinLine* Line;
    public int f14;
    public Bool32 maybeLeft;
    public ushort WallPointIndex;
    public ushort gap1E;
    public Bool32 IsNotReallyVisible;
    public CMerlinBSP* BSP;
    public CTracedPoint* Behind;
    public CTracedPoint* Before;

    static CTracedPoint()
    {
        Debug.Assert(sizeof(CTracedPoint) == 0x30);
    }
}


[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal unsafe partial struct CRenderer
{
    private static readonly CGdiObject* GlobalPalette = (CGdiObject*)0x004A1D50;

    public CRendererInner* Inner;
    public void* BmData;
    public int f8;
    public CBitmap Bitmap;
    public int Width, Height;
    public int F1C;
    public CMerlinWorld* World;
    public int F24, F28;
    public int NegativeWidth;
    public int AngleToWidthScale;
    public int angle34, f38;
    public CWallRenderPoint* PerBSP;
    public CWallRenderPoint** PerWidth;
    public CTracedPoint* TracedPoints;

    static CRenderer()
    {
        Debug.Assert(sizeof(CRenderer) == 0x48);
    }

    [Attach(0x004191B0)]
    public CRenderer* Ctor(int width, int height, CMerlinWorld* world)
    {
        Bitmap.Ctor();
        Width = width & ~3;
        Height = height;
        InitBitmap(GlobalPalette);
        NegativeWidth = -width;
        F1C = -1;
        F24 = 1;
        fixed (CRenderer* pThis = &this) Inner = Globals.New<CRendererInner>()->Ctor(pThis);
        PerBSP = null;
        PerWidth = null;
        TracedPoints = null;
        Init(world);

        fixed (CRenderer* pThis = &this) return pThis;
    }

    [Attach(0x00419440)]
    public partial void InitBitmap(CGdiObject* palette);

    [Attach(0x004192F0)]
    public void Init(CMerlinWorld* world)
    {
        Globals.Delete(PerBSP);
        Globals.Delete(PerWidth);
        Globals.Delete(TracedPoints);

        var bspCount = world->Bsps.Size + 100;
        PerBSP = Globals.New<CWallRenderPoint>(2 * bspCount);
        PerWidth = (CWallRenderPoint**)Globals.New<IntPtr>(Width);
        TracedPoints = Globals.New<CTracedPoint>(4 * bspCount);
    }
}