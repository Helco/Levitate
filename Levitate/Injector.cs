using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Levitate
{
    public static class Injector
    {
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int MessageBox(int hWnd, string text, string caption, uint type);

        public static int Inject(IntPtr argPtr, int argLen)
        {
            Console.WriteLine("Hello World");
            Debug.WriteLine("Hello Debug");
            MessageBox(0, "Hello message", "", 0);
            return 0;
        }
    }
}