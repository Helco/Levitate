using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Levitate.Game;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal unsafe partial struct CRendererInner
{
    public int height;
    public int f4, f8, fC, f10, f14;
    public void* muchData, muchDataAgain;

    static CRendererInner()
    {
        Debug.Assert(sizeof(CRendererInner) == 0x20);
    }

    [Attach(0x0041B870)]
    public partial CRendererInner* Ctor(CRenderer* renderer);
}
