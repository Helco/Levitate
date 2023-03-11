using System;
using System.Text;
using System.Runtime.InteropServices;

namespace Levitate.Mfc
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal unsafe partial struct CString
    {
        public static readonly byte* EmptyData = (byte*)0x4C695C;

        public byte* Data;
        public int Length;
        public int Capacity;

        [Attach(0x44B986)]
        public partial CString* Ctor();

        [Attach(0x44B9DE)]
        public partial void Empty();

        [Attach(0x44BD57)]
        public partial byte* GetBufferSetLength(int length);

        [Attach(0x0044BB24)]
        public partial void SetFromUnicode(char* text);

        public override string ToString()
        {
            if (Data == null || Length < 0)
                return "<null>";
            return Encoding.UTF8.GetString(Data, Length);
        }
    }
}
