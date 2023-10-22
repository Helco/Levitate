using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Levitate.Game;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal unsafe partial struct CRendererImpl
{
    public int f0, f4, f8, fC, f10, f14;
    public void* func18, func1C, func20, func24, func28, func2C, func30, func34, func38, func3C, func40;

    static CRendererImpl()
    {
        Debug.Assert(sizeof(CRendererImpl) == 0x44);
    }
}
