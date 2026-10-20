using System.Runtime.InteropServices;

namespace Hosihikari.Bindings.Abi;

/// <summary>
/// One entry of a returned map. String keys/values are borrowed UTF-8 buffers;
/// owned record/variant values are addresses of shim-owned copies.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct MapEntry
{
    public ulong Key;
    public ulong KeySize;
    public ulong Value;
    public ulong ValueSize;
}
