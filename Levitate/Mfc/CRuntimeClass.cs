using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Levitate.Mfc;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal unsafe partial struct CRuntimeClass
{
    public byte* NameRaw;
    public string? Name => Marshal.PtrToStringUTF8((nint)NameRaw);
    public int Size;
    public int Schema;
    public delegate* unmanaged[Stdcall]<CObject*, CObject*> CreateObject;
    public CRuntimeClass* Base;
    public CRuntimeClass* Next;

    public bool IsInstance(CObject* o)
    { 
        fixed (CRuntimeClass* pThis = &this)
            return o->IsKindOf(pThis);
    }

    [Attach(0x0044E7F3)]
    public partial bool ConstructObject(CObject* o);

    [Attach(0x0044E733)]
    public partial CObject* Create();
}
