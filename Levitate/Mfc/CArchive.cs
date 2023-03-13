using System;
using System.Runtime.InteropServices;

namespace Levitate.Mfc;

[Flags]
internal enum ArchiveMode : uint
{
    Load = 1
}

[StructLayout(LayoutKind.Explicit)]
internal unsafe partial struct CArchive
{
    [FieldOffset(0xC)] public ArchiveMode Mode;
    [FieldOffset(0x1C)] public byte* Cur;
    [FieldOffset(0x20)] public byte* Max;
    [FieldOffset(0x28)] public int CurrentSchema;
    [FieldOffset(0x2C)] public int TagCount;
    [FieldOffset(0x30)] public CPtrArray* TagLookup;

    [Attach(0x00452C38)]
    public partial void FillBuffer(uint size);

    [Attach(0x00452A1E)]
    public partial int Read(byte* buffer, int size);

    public void EnsureBuffer(int size)
    {
        if (Cur + size > Max)
            FillBuffer((uint)(Cur + size - Max));
    }

    public byte ReadByte() => ReadBlit<byte>();
    public short ReadShort() => ReadBlit<short>();
    public ushort ReadUShort() => ReadBlit<ushort>();
    public int ReadInt() => ReadBlit<int>();
    public uint ReadUInt() => ReadBlit<uint>();
    public float ReadFloat() => ReadBlit<float>();

    public T ReadBlit<T>() where T : unmanaged
    {
        EnsureBuffer(sizeof(T));
        T value = *(T*)Cur;
        Cur += sizeof(T);
        return value;
    }

    [Attach(0x004526AF, CallingConvention.StdCall)]
    public int ReadStringLength()
    {
        var byteLen = ReadByte();
        if (byteLen != 0xff)
            return byteLen;

        var shortLen = ReadUShort();
        if (shortLen == 0xFFFE)
            return -1;
        if (shortLen != 0xFFFF)
            return shortLen;

        return ReadInt();
    }

    [Attach(0x00452750, CallingConvention.StdCall)]
    public void ReadString(CString* str)
    {
        int length = ReadStringLength();
        int charSize = 1;
        if (length == -1)
        {
            charSize = 2;
            length = ReadStringLength();
        }

        var buffer = str->GetBufferSetLength(length);
        if (length == 0)
            return;

        if (Read(buffer, length * charSize) != length * charSize)
            throw new EndOfStreamException();
        if (charSize == 1)
            return;

        str->Data = CString.EmptyData;
        buffer[length * charSize + 0] = 0; // TODO: looks like a possible out-of-bounds write
        buffer[length * charSize + 1] = 0;
        str->SetFromUnicode((char*)buffer);
        Globals.Delete(buffer);
    }

    public void ReadString(out CString str)
    {
        fixed (CString* strPtr = &str)
            ReadString(strPtr);
    }

    public void Skip(int size)
    {
        EnsureBuffer(size);
        Cur += size;
    }

    public const int MaxClassNameLength = 64;
    [Attach(0x004527E9, CallingConvention.StdCall)]
    public CRuntimeClass* ReadClass(int* dataSchema)
    {
        *dataSchema = ReadUShort();
        var nameLen = ReadUShort();
        if (nameLen >= MaxClassNameLength)
            return null;

        var name = stackalloc byte[nameLen + 1];
        if (Read(name, nameLen) != nameLen)
            return null;
        name[nameLen] = 0;

        return CRuntimeClass.ByName(Marshal.PtrToStringUTF8((IntPtr)name, nameLen));
    }

    public CRuntimeClass* ReadClass(out int dataSchema)
    {
        fixed (int* dataSchemaPtr = &dataSchema)
            return ReadClass(dataSchemaPtr);
    }

    [Attach(0x00453750)]
    public CObject* ReadObject(CRuntimeClass* baseClass)
    {
        if (baseClass != null && baseClass->Schema == 0xFFFF)
            Globals.ThrowNoMfcSerialization();

        if (TagLookup == null)
        {
            TagLookup = Globals.New<CPtrArray>();
            TagLookup->Ctor();
            TagLookup->SetSize(64, 64);
            TagLookup->Data[0] = IntPtr.Zero;
            TagCount = 1;
        }

        short tag = ReadShort();
        if (tag >= 0)
        {
            if (tag >= TagLookup->Size)
                Globals.AfxThrowArchiveException(5);
            var oldObject = (CObject*)TagLookup->Data[tag];
            if (oldObject != null && baseClass != null && !baseClass->IsInstance(oldObject))
                Globals.AfxThrowArchiveException(6);
            return oldObject;
        }

        CRuntimeClass* actualClass;
        int dataSchema = 0; // actually undefined for looked-up classes
        if (tag == -1)
        {
            if (TagCount >= 0x7FFF)
                Globals.AfxThrowArchiveException(5);
            actualClass = ReadClass(out dataSchema);
            if (actualClass == null)
                Globals.AfxThrowArchiveException(6);
            if (actualClass->Schema >= 0 && actualClass->Schema != dataSchema)
                Globals.AfxThrowArchiveException(7);

            TagLookup->InsertAt(TagCount++, (IntPtr)actualClass);
        }
        else
        {
            tag &= 0x7FFF;
            if (tag >= TagLookup->Size)
                Globals.AfxThrowArchiveException(5);
            actualClass = (CRuntimeClass*)TagLookup->Data[tag];
        }

        var newObject = actualClass->Create();
        if (newObject == null)
            Globals.ThrowNoMfcObjectAlloc();
        TagLookup->InsertAt(TagCount++, (IntPtr)newObject);

        var backupSchema = CurrentSchema;
        CurrentSchema = dataSchema;
        fixed (CArchive* pThis = &this)
            newObject->VtPtr->Serialize(newObject, pThis);
        CurrentSchema = backupSchema;

        if (baseClass != null && !baseClass->IsInstance(newObject))
            Globals.AfxThrowArchiveException(6);
        return newObject;
    }

    public CObject* ReadObject() => ReadObject(null);
    public T* ReadObject<T>() where T : unmanaged, IRuntimeObject => (T*)ReadObject(T.RuntimeClass);
}
