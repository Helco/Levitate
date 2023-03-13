using System;
using System.Runtime.InteropServices;
using Levitate.Mfc;

namespace Levitate.Merlin;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal unsafe partial struct CMerlinTexture
{
    public static readonly CObject.VTable* VirtualTable = (CObject.VTable*)0x004BBD20;
    public static readonly CMapStringToOb* ByName = (CMapStringToOb*)0x004A1D30;
    public static readonly CObArray* UnusedArray = (CObArray*)0x004A1D08;
    public static readonly int* AliveCount = (int*)0x004C4DA0;
    public static readonly byte** FixedMemoryCur = (byte**)0x00461D00;

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct PixelSpan
    {
        public ushort From, To;
        public byte* Data;

        public void Read(CArchive* arc, int mipIndex, ref byte* nextData)
        {
            From = arc->ReadUShort();
            To = arc->ReadUShort();
            Data = nextData;
            nextData += (To >> mipIndex) - (From >> mipIndex) + 1;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct RowSpan
    {
        public int PixelSpanCount;
        public PixelSpan* PixelSpans;

        public void Read(CArchive* arc, int mipIndex, ref PixelSpan* nextPixelSpan, ref byte* nextData)
        {
            PixelSpanCount = arc->ReadUShort();
            PixelSpans = nextPixelSpan;
            nextPixelSpan += PixelSpanCount;

            for (int i = 0; i < PixelSpanCount; i++)
                PixelSpans[i].Read(arc, mipIndex, ref nextData);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct Mipmap
    {
        public int MaxY, MaxX;
        public int Height, Width;
        public int Level;
        public int DataSize;
        public byte* Data;
        public RowSpan* RowSpans;

        public void Read(CArchive* arc, int mipIndex)
        {
            Height = arc->ReadUShort();
            MaxY = arc->ReadUShort();
            Width = arc->ReadUShort();
            MaxX = arc->ReadUShort();
            Level = arc->ReadUShort(); // might be different from mipIndex
            DataSize = arc->ReadInt();

            // we ignore the low-memory handling and always read everything
            Data = *FixedMemoryCur;
            *FixedMemoryCur += DataSize;
            arc->Read(Data, DataSize);

            RowSpans = (RowSpan*)*FixedMemoryCur;
            *FixedMemoryCur += sizeof(RowSpan) * Height;
            var nextPixelSpan = (PixelSpan*)*FixedMemoryCur;
            *FixedMemoryCur += sizeof(PixelSpan) * arc->ReadInt();
            var nextData = Data;

            for (int y = 0; y < Height; y++)
                RowSpans[y].Read(arc, mipIndex, ref nextPixelSpan, ref nextData);

            arc->Skip(arc->ReadUShort());
        }
    }

    public CMerlinObject @base;
    public ushort Height, MaxY, Width, MaxX;
    public uint IsTransparent;
    public byte* Data;
    public int unk1;
    public Mipmap* Mipmaps;
    public int MipmapCount;
    public int unk2, unk3, unk4;

    public void Ctor()
    {
        @base.Ctor();
        @base.VtPtr = VirtualTable;
        Height = Width = 0;
        Data = null;
        Mipmaps = null;
        MipmapCount = 0;
        IsTransparent = 0;
        unk1 = unk2 = unk3 = unk4 = 0;
    }

    [Attach(0x4128F0, CallingConvention.StdCall)]
    public static CMerlinTexture* Create(CMerlinTexture* texture)
    {
        if (texture != null)
            texture->Ctor();
        return texture;
    }

    [Attach(0x412980)]
    public void Serialize(CArchive* arc)
    {
        @base.Serialize(arc);
        if (!arc->Mode.HasFlag(ArchiveMode.Load))
            throw new NotImplementedException();

        fixed (CMerlinTexture* pThis = &this)
            *ByName->GetOrCreate(@base.Name.Data) = (CObject*)pThis;

        IsTransparent = arc->ReadUShort();
        MipmapCount = arc->ReadUShort();
        Mipmaps = (Mipmap*)*FixedMemoryCur;
        *FixedMemoryCur += sizeof(Mipmap) * MipmapCount;
        for (int i = 0; i < MipmapCount; i++)
            Mipmaps[i].Read(arc, i);

        Height = (ushort)Mipmaps->Height;
        Width = (ushort)Mipmaps->Width;
        MaxY = (ushort)Mipmaps->MaxY;
        MaxX = (ushort)Mipmaps->MaxX;
        Data = Mipmaps->Data;
        unk1 = unk3 = 0;

        if (Globals.ProgressCallback != null)
        {
            Globals.ProgressCallback(*Globals.ProgressNum, UnusedArray->Size - 1);
            ++*Globals.ProgressNum;
        }
    }
}
