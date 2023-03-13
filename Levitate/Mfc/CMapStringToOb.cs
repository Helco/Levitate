using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Levitate.Mfc;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal unsafe partial struct CMapStringToOb
{
    [Attach(0x00449F9B)]
    public partial CObject** GetOrCreate(byte* key);

    public ref CObject* GetOrCreate(in CString key) => ref GetOrCreate(key);

    [Attach(0x00449F6E)]
    public partial bool TryGetValue(byte* key, CObject** value);

    public bool TryGetValue(in CString key, out CObject* value)
    {
        fixed (CObject** valueAddr = &value)
            return TryGetValue(key.Data, valueAddr);
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal unsafe struct CMapStringToOb<T> where T : unmanaged
{
    public CMapStringToOb Raw;

    public ref T* GetOrCreate(byte* key) => ref *(T**)Raw.GetOrCreate(key);

    public ref T* GetOrCreate(in CString key) => ref GetOrCreate(key.Data);

    public void Set(in CString key, in T value)
    {
        fixed (T* valuePtr = &value)
            GetOrCreate(key) = valuePtr;
    }

    public bool TryGetValue(byte* key, out T* value)
    {
        fixed (T** valueAddr = &value)
            return Raw.TryGetValue(key, (CObject**)valueAddr);
    }

    public bool TryGetValue(in CString key, out T* value) => TryGetValue(key.Data, out value);
}
