using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Levitate.Mfc;

namespace Levitate.Merlin;

// does not occur in the original mazes

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal unsafe partial struct CMerlinDynamic : IRuntimeObject
{
    public static CRuntimeClass* RuntimeClass => (CRuntimeClass*)0x004C5608;
    public static readonly CObject.VTable* VirtualTable = (CObject.VTable*)0x004BD348;
    public static readonly CMapStringToOb<CMerlinDynamic>* ByName = (CMapStringToOb<CMerlinDynamic>*)0x004A4728;

    public CMerlinLine @base;
    public CString TexName;
    public CMerlinTexture* Texture;
    public short unk54, unk56, unk58, unk5A;
    public int unk5C;
    public int unk60;
    public int f64, f68, f6C;

    public void Ctor()
    {
        @base.CtorEmpty();
        @base.@base.VtPtr = VirtualTable;
        TexName.Ctor();
        Texture = null;
        unk54 = 0;
        unk56 = 128;
        unk60 = 1;
        unk58 = 128;
        f64 = 1;
        unk5C = 0;
        unk5A = 0;
        f68 = f6C = 0;
    }

    [Attach(0x0042CAB0, CallingConvention.StdCall)]
    public static CMerlinDynamic* Create(CMerlinDynamic* dyn)
    {
        if (dyn != null)
            dyn->Ctor();
        return dyn;
    }

    [Attach(0x0042CC40)]
    public void Serialize(CArchive* arc)
    {
        @base.Serialize(arc);
        if (!arc->Mode.HasFlag(ArchiveMode.Load))
            throw new NotImplementedException();

        if (@base.@base.Name.Length > 0)
            ByName->Set(@base.@base.Name, this);

        arc->ReadString(out TexName);
        Texture = null;
        if (TexName.Length > 0)
            CMerlinTexture.ByName->TryGetValue(TexName, out Texture);

        unk54 = arc->ReadShort();
        unk56 = arc->ReadShort();
        unk58 = arc->ReadShort();
        unk5A = arc->ReadShort();
        unk5C = arc->ReadShort();
        unk60 = arc->ReadByte();

        arc->Skip(arc->ReadUShort());
        Debug.WriteLine(@base.@base.Name);
    }
}
