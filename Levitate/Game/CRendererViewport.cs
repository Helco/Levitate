using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Levitate.Utils;

namespace Levitate.Game;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal unsafe partial struct CRendererViewport
{
    public int f0, f4;
    public CRendererImpl* funcs;
    public int fC, f10;
    public void* bitcounts_14;
    public int f18, f1C, f20, f24, f28, f2C;
    public void* obj30, obj34, obj38;
    public int modeSetting1, modeSetting2;
    public int Quality;
    public IVector2 Pos;
    public int CamHeight;
    public int Angle;
    public int f58;
    public void* obj5C;
    public int f60;
    public void* obj64;
    public int f68;
    public void* obj6C;
    public int f70, f74, f78, f7C;
    public Bool32 isFlag2;
    public int f84, f88, f8C;
    public Bool32 isFlag1;
    public int f94, f98;
    public float ff9C;
    public int fA0;
    public float ffA4;
    public int LastHeight;
    public int fAC;

    static CRendererViewport()
    {
        Debug.Assert(sizeof(CRendererViewport) == 0xB0);
    }

    [Attach(0x00420ED0)]
    public CRendererViewport* Ctor()
    {
        fA0 = 0;
        f98 = 0;
        ffA4 = 3.25f;
        ff9C = 3.25f;
        f2C = 0;
        f58 = 128;
        LastHeight = -1;
        CamHeight = 64;
        SetFlagsAndQuality(1, 1);
        obj30 = obj34 = obj38 = null;
        obj5C = obj64 = obj6C = null;
        f68 = 0;
        bitcounts_14 = Globals.New(0x800);
        funcs = Globals.New<CRendererImpl>();

        fixed (CRendererViewport* pThis = &this) return pThis;
    }

    [Attach(0x00420F60)]
    public void Dtor()
    {
        Globals.Delete(funcs);
        Globals.Delete(bitcounts_14);
        Globals.Delete(obj30);
        Globals.Delete(obj34);
        Globals.Delete(obj38);
        Globals.Delete(obj5C);
        Globals.Delete(obj64);
        Globals.Delete(obj6C);
    }

    [Attach(0x00410F20)]
    public partial void SetFlagsAndQuality(byte flags, byte quality);
}
