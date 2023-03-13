﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Levitate.Mfc;

namespace Levitate.Merlin;

// does not occur in the original mazes

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal unsafe partial struct CMerlinDynamic
{
    public static readonly CMerlinObject.VTable* VirtualTable = (CMerlinObject.VTable*)0x004BD348;
    public static readonly CMapStringToOb* ByName = (CMapStringToOb*)0x004A4728;

    public CMerlinLine @base;
    public CString texName;
    public CMerlinTexture* texture;
    public short unk54, unk56, unk58, unk5A;
    public int unk5C;
    public int unk60;
    public int f64, f68, f6C;

    public void Ctor()
    {
        @base.CtorEmpty();
        @base.@base.VtPtr = VirtualTable;
        texName.Ctor();
        texture = null;
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

        fixed (CMerlinDynamic* pThis = &this)
        {
            if (@base.@base.Name.Length > 0)
                *ByName->GetOrCreate(@base.@base.Name.Data) = (CObject*)pThis;

            arc->ReadString(&pThis->texName);
            texture = null;
            if (texName.Length > 0)
                CMerlinTexture.ByName->TryGetValue(texName.Data, (CObject**)&pThis->texture);
        }

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
