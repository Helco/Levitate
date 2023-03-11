using System;
using System.Runtime.InteropServices;
using Levitate.Mfc;

namespace Levitate.Merlin
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal unsafe partial struct CMerlinObject
    {
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public unsafe struct VTable
        {
            IntPtr GetRuntimeClass;
            IntPtr Dtor;
            IntPtr Serialize;
        }
        public static readonly VTable* VirtualTable = (VTable*)0x4BD808;

        public VTable* VtPtr;
        public Mfc.CString Name;
        public int unknown;

        [Attach(0x401030)]
        public CMerlinObject* Ctor()
        {
            VtPtr = VirtualTable;
            Name.Ctor();
            unknown = 0;
            fixed (CMerlinObject* pThis = &this) return pThis;
        }

        [Attach(0x00411B10)]
        public void Serialize(CArchive* archive)
        {
            if (archive->mode.HasFlag(CArchive.Mode.Load))
            {
                fixed (CMerlinObject* pThis = &this)
                    archive->ReadString(&pThis->Name);
                archive->Skip(archive->ReadUShort());
            }
            else
                throw new NotImplementedException();
        }
    }
}
