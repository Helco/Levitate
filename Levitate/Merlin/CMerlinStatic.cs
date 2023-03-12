using System;
using System.Runtime.InteropServices;
using Levitate.Mfc;

namespace Levitate.Merlin;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal unsafe partial struct CMerlinStatic
{
    public static readonly CMerlinObject.VTable* VirtualTable = (CMerlinObject.VTable*)0x004BC078;
    public static readonly CMapStringToOb* ByName = (CMapStringToOb*)0x004A4578;

    public CMerlinLine @base;
    public CString
        texNameLT,
        texNameRT,
        texNameLW,
        texNameRW,
        texNameLB,
        texNameRB;
    public CMerlinTexture*
        textureLT,
        textureRT,
        textureLW,
        textureRW,
        textureLB,
        textureRB;
    public short bottomZ, topZ;
    public short unk1, unk2;
    public int unk3;
    public bool texLWIsTransparent, texRWIsTransparent;
    public short leftTexOffset, rightTexOffset;
    public bool isSolid;
    public bool unkFlag;

    [Attach(0x00419B30)]
    public CMerlinStatic* Ctor()
    {
        @base.CtorEmpty();
        @base.@base.VtPtr = VirtualTable;
        texNameLT.Ctor();
        texNameRT.Ctor();
        texNameLW.Ctor();
        texNameRW.Ctor();
        texNameLB.Ctor();
        texNameRB.Ctor();
        texLWIsTransparent = false;
        texRWIsTransparent = false;
        leftTexOffset = rightTexOffset = 0;
        bottomZ = 0;
        topZ = 128;
        unk1 = unk2 = 0;
        unk3 = 0;
        isSolid = true;
        unkFlag = false;
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

                LoadTexture(arc, &pThis->texNameLT, &pThis->textureLT);
                LoadTexture(arc, &pThis->texNameLT, &pThis->textureRT);
                LoadTexture(arc, &pThis->texNameLW, &pThis->textureLW);
                LoadTexture(arc, &pThis->texNameLW, &pThis->textureRW);
                LoadTexture(arc, &pThis->texNameLB, &pThis->textureLB);
                LoadTexture(arc, &pThis->texNameLB, &pThis->textureRB);
            }

            bottomZ = arc->ReadShort();
            topZ = arc->ReadShort();
            unk1 = arc->ReadShort();
            unk2 = arc->ReadShort();
            texLWIsTransparent = arc->ReadByte() != 0;
            texRWIsTransparent = arc->ReadByte() != 0;
            isSolid = arc->ReadByte() != 0;

            var skipBytes = arc->ReadUShort();
            if (skipBytes >= 5)
            {
                skipBytes -= 5;
                unkFlag = arc->ReadByte() != 0;
                leftTexOffset = arc->ReadShort();
                rightTexOffset = arc->ReadShort();
            }
            arc->Skip(skipBytes);
        }
        else
            throw new NotImplementedException();
    }

    private static void LoadTexture(CArchive* arc, CString* texName, CMerlinTexture** texture)
    {
        arc->ReadString(texName);
        if (texName->Length == 0 ||
            !CMerlinTexture.ByName->TryGetValue(texName->Data, (CObject**)texture))
            *texture = null;
    }
}
