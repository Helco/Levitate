using System.Diagnostics;
using System.Runtime.InteropServices;
using Levitate.Mfc;
using static Levitate.Detours;

namespace Levitate
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AttachAttribute : Attribute
    {
        public AttachAttribute(uint address, CallingConvention callConv)
        {
            Address = address;
        }

        public uint Address { get; }
        public CallingConvention CallConv { get; }
    }

    public static unsafe partial class Injector
    {
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int MessageBox(int hWnd, string text, string caption, uint type);

        public static int Inject(IntPtr argPtr, int argLen)
        {
            CheckWin(DetourTransactionBegin());
            var fn = CArchive.OrigReadStringLength;
            var fn2 = fn;
            fn2 = &CArchive.UnReadStringLength;
            CheckWin(DetourAttach(new(&fn), new(fn2)));
            AttachAllMethods();
            CheckWin(DetourTransactionCommit());
            Debug.WriteLine("Done injecting");
            return 0;
        }

        private static partial void AttachAllMethods();
    }
}