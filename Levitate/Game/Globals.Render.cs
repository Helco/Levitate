using System;
using Levitate.Game;
using Levitate.Merlin;
using Levitate.Utils;

namespace Levitate;

unsafe partial struct Globals
{
    public static readonly CRendererViewport** viewport = (CRendererViewport**)0x004A1D68;
    public static readonly int* vp2C = (int*)0x4A1D70;
    public static readonly int* ren24 = (int*)0x4A1D78;
    public static readonly CMerlinLine* renLineLeft = (CMerlinLine*)0x4A4498;
    public static readonly CMerlinLine* renLineRight = (CMerlinLine*)0x4A44E0;
    public static readonly short* wallRenderCount = (short*)0x4A4528;
    public static readonly int* vp5C = (int*)0x4A452C;
    public static readonly int* itX = (int*)0x4A4534;
    public static readonly int* modeSetting2Bits = (int*)0x4A4538;
    public static readonly int* modeSetting2 = (int*)0x4A453C;
    public static readonly int* camHeight = (int*)0x4A4544;
    public static readonly void** bitCounts = (void**)0x4A4548;
    public static readonly int* vpWidth = (int*)0x4A454C;
    public static readonly CWallRenderPoint** nextWallRenderData = (CWallRenderPoint**)0x4A4550;
    public static readonly int* modeSetting1Bits = (int*)0x4A4554;
    public static readonly Bool32* isCurDynamic = (Bool32*)0x4A4558;
    public static readonly int* modeSetting1 = (int*)0x4A455C;
    public static readonly int* vp7C = (int*)0x4A4A30;
    public static readonly int* renNegativeWidth = (int*)0x4A4A34;
    public static readonly CTracedPoint** nextOrderedTraced = (CTracedPoint**)0x4A4714;
    public static readonly CTracedPoint** nextTracedPointMem = (CTracedPoint**)0x4A4718;
}
