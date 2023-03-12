using System;

namespace Levitate.Mfc;

internal unsafe partial struct CMapStringToOb
{
    [Attach(0x00449F9B)]
    public partial CObject** Lookup(byte* key);

    public CObject** Lookup(CString* key) => Lookup(key->Data);
}
