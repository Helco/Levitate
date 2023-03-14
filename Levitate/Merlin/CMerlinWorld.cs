using System;
using System.Runtime.InteropServices;
using Levitate.Mfc;

namespace Levitate.Merlin;

[StructLayout(LayoutKind.Sequential)]
internal unsafe partial struct CMerlinWorld
{
    public static readonly CObject.VTable* VirtualTable = (CObject.VTable*)0x004BBEC8;

    public CObject.VTable* VtPtr;
    public int MinX, MinY, MaxX, MaxY;
    public CObArray<CMerlinStatic> Statics;
    public CObArray<CMerlinDynamic> Dynamics;
    public CObArray<CMerlinLocation> Locations;
    public CObArray<CMerlinBSP> Bsps;

    [Attach(0x00418AE0)]
    public CMerlinWorld* Ctor()
    {
        VtPtr = VirtualTable;
        Statics.Ctor();
        Dynamics.Ctor();
        Locations.Ctor();
        Bsps.Ctor();

        if (CMerlinTexture.UnusedArray->Size > 0)
            throw new NotImplementedException();

        fixed (CMerlinWorld* pThis = &this) return pThis;
    }

    [Attach(0x004298E0, CallingConvention.StdCall)]
    public static CMerlinWorld* Create(CMerlinWorld* world)
    {
        if (world != null)
            world->Ctor();
        return world;
    }

    [Attach(0x00418DC0)]
    public void Serialize(CArchive* arc)  
    {
        if (!arc->Mode.HasFlag(ArchiveMode.Load))
            throw new NotImplementedException();

        // not too sure on this order
        MinY = arc->ReadShort();
        MinX = arc->ReadShort();
        MaxX = arc->ReadShort();
        MaxY = arc->ReadShort();

        Statics.Serialize(arc);
        Dynamics.Serialize(arc);
        Locations.Serialize(arc);
        Bsps.Serialize(arc);
    }
}
