using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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

        [Attach(0x00452C38, CallingConvention.ThisCall)]
        public partial void FillBuffer(uint size);

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

        public static readonly delegate* unmanaged[Stdcall]<CArchive*, int> OrigReadStringLength = (delegate* unmanaged[Stdcall]<CArchive*, int>)0x004526AF;

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        public static int UnReadStringLength(CArchive* archive) => archive->ReadStringLength();
    }
}
