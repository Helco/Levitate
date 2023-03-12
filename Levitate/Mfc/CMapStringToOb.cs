using System;

namespace Levitate.Mfc;

internal unsafe partial struct CMapStringToOb
{
    [Attach(0x00449F9B)]
    public partial CObject** GetOrCreate(byte* key);

    [Attach(0x00449F6E)]
    public partial bool TryGetValue(byte* key, CObject** value);

    public CObject** GetOrCreate(CString* key) => GetOrCreate(key->Data);
}
