using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Levitate.Mfc
{
    [StructLayout(LayoutKind.Explicit)]
    internal unsafe partial struct CArchive
    {
        [Flags]
        public enum Mode : uint
        {
            Load = 1
        }

        [FieldOffset(0xC)] public Mode mode;
        [FieldOffset(0x1C)] public byte* cur;
        [FieldOffset(0x20)] public byte* max;

        [Attach(0x00452C38)]
        public partial void FillBuffer(uint size);

        [Attach(0x00452A1E)]
        public partial int Read(byte* buffer, int size);

        public void EnsureBuffer(int size)
        {
            if (cur + size > max)
                FillBuffer((uint)(cur + size - max));
        }

        public byte ReadByte() => ReadBlit<byte>();
        public ushort ReadUShort() => ReadBlit<ushort>();
        public uint ReadUInt() => ReadBlit<uint>();
        public int ReadInt() => ReadBlit<int>();

        public T ReadBlit<T>() where T : unmanaged
        {
            EnsureBuffer(sizeof(T));
            T value = *(T*)cur;
            cur += sizeof(T);
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

        public void Skip(int size)
        {
            EnsureBuffer(size);
            cur += size;
        }
    }
}
