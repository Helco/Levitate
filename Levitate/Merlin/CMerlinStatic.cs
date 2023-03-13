using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Levitate.Mfc;

namespace Levitate.Merlin;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal unsafe partial struct CMerlinStatic
{
    public static readonly CObject.VTable* VirtualTable = (CObject.VTable*)0x004BC078;
    public static readonly CMapStringToOb* ByName = (CMapStringToOb*)0x004A4578;

    public CMerlinLine @base;
    public CString
        TexNameLT,
        TexNameRT,
        TexNameLW,
        TexNameRW,
        TexNameLB,
        TexNameRB;
    public CMerlinTexture*
        TextureLT,
        TextureRT,
        TextureLW,
        TextureRW,
        TextureLB,
        TextureRB;
    public short BottomZ, TopZ;
    public short unk1, unk2;
    public int unk3;
    public uint TexLWIsTransparent, TexRWIsTransparent;
    public short LeftTexOffset, RightTexOffset;
    public uint IsSolid;
    public uint unkFlag;

    [Attach(0x00419B30)]
    public CMerlinStatic* Ctor()
    {
        @base.CtorEmpty();
        @base.@base.VtPtr = VirtualTable;
        TexNameLT.Ctor();
        TexNameRT.Ctor();
        TexNameLW.Ctor();
        TexNameRW.Ctor();
        TexNameLB.Ctor();
        TexNameRB.Ctor();
        TextureLT = TextureRT = TextureLW = TextureRW = TextureLB = TextureRB = null;
        TexLWIsTransparent = 0;
        TexRWIsTransparent = 0;
        LeftTexOffset = RightTexOffset = 0;
        BottomZ = 0;
        TopZ = 128;
        unk1 = unk2 = 0;
        unk3 = 0;
        IsSolid = 1;
        unkFlag = 0;
        fixed (CMerlinStatic* pThis = &this) return pThis;
    }

    [Attach(0x00419DE0, CallingConvention.StdCall)]
    public static CMerlinStatic* Create(CMerlinStatic* stat)
    {
        if (stat != null)
            stat->Ctor();
        return stat;
    }

    [Attach(0x00419E00)]
    public void Serialize(CArchive* arc)
    {
        @base.Serialize(arc);
        if (arc->Mode.HasFlag(ArchiveMode.Load))
        {
            fixed (CMerlinStatic* pThis = &this)
            {
                if (@base.@base.Name.Length > 0)
                    *ByName->GetOrCreate(@base.@base.Name.Data) = (CObject*)pThis;

                LoadTexture(arc, &pThis->TexNameLT, &pThis->TextureLT);
                LoadTexture(arc, &pThis->TexNameRT, &pThis->TextureRT);
                LoadTexture(arc, &pThis->TexNameLW, &pThis->TextureLW);
                LoadTexture(arc, &pThis->TexNameRW, &pThis->TextureRW);
                LoadTexture(arc, &pThis->TexNameLB, &pThis->TextureLB);
                LoadTexture(arc, &pThis->TexNameRB, &pThis->TextureRB);
            }

            BottomZ = arc->ReadShort();
            TopZ = arc->ReadShort();
            unk1 = arc->ReadShort();
            unk2 = arc->ReadShort();
            TexLWIsTransparent = arc->ReadByte();
            TexRWIsTransparent = arc->ReadByte();
            IsSolid = arc->ReadByte();

            var skipBytes = arc->ReadUShort();
            if (skipBytes >= 5)
            {
                skipBytes -= 5;
                unkFlag = arc->ReadByte();
                LeftTexOffset = arc->ReadShort();
                RightTexOffset = arc->ReadShort();
            }
            arc->Skip(skipBytes);
        }
        else
            throw new NotImplementedException();
    }

    private static void LoadTexture(CArchive* arc, CString* texName, CMerlinTexture** texture)
    {
        arc->ReadString(texName);
        *texture = null;
        if (texName->Length != 0)
            CMerlinTexture.ByName->TryGetValue(texName->Data, (CObject**)texture);
    }
}
