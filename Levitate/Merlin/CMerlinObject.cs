using System;
using System.Runtime.InteropServices;
using Levitate.Mfc;

namespace Levitate.Merlin;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal unsafe partial struct CMerlinObject
{
    public static readonly CObject.VTable* VirtualTable = (CObject.VTable*)0x4BD808;

    public CObject.VTable* VtPtr;
    public CString Name;
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
        if (archive->Mode.HasFlag(ArchiveMode.Load))
        {
            fixed (CMerlinObject* pThis = &this)
                archive->ReadString(&pThis->Name);
            archive->Skip(archive->ReadUShort());
        }
        else
            throw new NotImplementedException();
    }
}
