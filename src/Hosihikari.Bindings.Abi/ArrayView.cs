using System.Runtime.InteropServices;

namespace Hosihikari.Bindings.Abi;

/// <summary>A borrowed array returned by a BindingShim.</summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct ArrayView
{
    public void* Data;
    public nuint Size;
}
