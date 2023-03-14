using System;
using System.Runtime.InteropServices;

namespace Levitate.Mfc;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal unsafe partial struct CObArray
{
    public IntPtr VtPtr;
    public CObject** Data;
    public int Size, Capacity, GrowBy;

    [Attach(0x004494CC)]
    public partial CObArray* Ctor();

    [Attach(0x00449598)]
    public partial void SetSize(int size, int growBy);
    public void SetSize(int size) => SetSize(size, -1);

    [Attach(0x00449724)]
    public void Serialize(CArchive* arc) => Serialize(arc, null);

    public void Serialize(CArchive* arc, CRuntimeClass* baseClass)
    {
        if (!arc->Mode.HasFlag(ArchiveMode.Load))
            throw new NotImplementedException();

        SetSize(arc->ReadUShort());
        for (int i = 0; i < Size; i++)
            Data[i] = arc->ReadObject(baseClass);
    }
}

#pragma warning disable CS9084 // Struct member returns 'this' or other instance members by reference
[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal unsafe struct CObArray<T> where T : unmanaged, IRuntimeObject
{
    public CObArray Raw;
    public T** Data => (T**)Raw.Data;
    public ref int Size => ref Raw.Size;
    public ref int Capacity => ref Raw.Capacity;

    public void Ctor() => Raw.Ctor();
    public void SetSize(int size, int growBy = -1) => Raw.SetSize(size, growBy);

    public void SerializeUnsafe(CArchive* arc) => Raw.Serialize(arc);

    public void Serialize(CArchive* arc) => Raw.Serialize(arc, T.RuntimeClass);
}
#pragma warning restore CS9084 // Struct member returns 'this' or other instance members by reference
