using System.Runtime.InteropServices;

namespace Hosihikari.Bindings.Abi;

/// <summary>The raw return shape of a C++ shared_ptr projection.</summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct SharedHandleData
{
    public void* Holder;
    public void* Target;
    public delegate* unmanaged[Cdecl]<void*, void> Release;
}
