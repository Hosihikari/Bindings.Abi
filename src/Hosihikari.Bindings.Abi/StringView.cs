using System.Runtime.InteropServices;

namespace Hosihikari.Bindings.Abi;

/// <summary>A borrowed UTF-8 buffer returned by a BindingShim.</summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct StringView
{
    public byte* Data;
    public nuint Size;
}
