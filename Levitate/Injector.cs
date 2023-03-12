using System.Diagnostics;
using System.Runtime.InteropServices;
using Levitate.Mfc;
using static Levitate.Detours;

namespace Levitate
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AttachAttribute : Attribute
    {
        public AttachAttribute(uint address, CallingConvention callConv = CallingConvention.ThisCall)
        {
            Address = address;
        }

        public uint Address { get; }
        public CallingConvention CallConv { get; }
    }

    public static unsafe partial class Injector
    {
        public static int Inject(IntPtr argPtr, int argLen)
        {
            AttachAllMethods();
            Debug.WriteLine("Injected levitation");
            return 0;
        }

        private static partial void AttachAllMethods();
    }
}