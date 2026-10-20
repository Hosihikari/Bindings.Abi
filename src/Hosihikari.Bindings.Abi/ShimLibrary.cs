using System.Runtime.InteropServices;

namespace Hosihikari.Bindings.Abi;

/// <summary>
/// Loads a BindingShim, validates its versioned function table, and owns the
/// native library handle for the lifetime of the process.
/// </summary>
public sealed unsafe class ShimLibrary
{
    private readonly nint _library;
    private readonly delegate* unmanaged[Cdecl]<byte*> _getLastError;

    private ShimLibrary(nint library, nint table, delegate* unmanaged[Cdecl]<byte*> getLastError)
    {
        _library = library;
        Table = table;
        _getLastError = getLastError;
    }

    public nint Table { get; }

    public static ShimLibrary Load(string nativeLibraryPath, uint expectedVersion, uint expectedSize)
    {
        var library = NativeLibrary.Load(nativeLibraryPath);
        try
        {
            var getTable = (delegate* unmanaged[Cdecl]<void*>)NativeLibrary
                .GetExport(library, "hosihikari_get_binding_table_v1");
            var getLastError = (delegate* unmanaged[Cdecl]<byte*>)NativeLibrary
                .GetExport(library, "hosihikari_get_last_error");
            var table = getTable();
            if (table is null)
            {
                throw new InvalidOperationException("The BindingShim returned a null function table.");
            }

            var version = *(uint*)table;
            if (version != expectedVersion)
            {
                throw new InvalidOperationException(
                    $"Unsupported BindingShim table version {version}; expected {expectedVersion}.");
            }

            var size = *((uint*)table + 1);
            if (size != expectedSize)
            {
                throw new InvalidOperationException(
                    $"BindingShim table size mismatch: native {size}, managed {expectedSize}.");
            }

            return new ShimLibrary(library, (nint)table, getLastError);
        }
        catch
        {
            NativeLibrary.Free(library);
            throw;
        }
    }

    public string? GetLastError()
    {
        var pointer = _getLastError();
        return pointer is null ? null : Marshal.PtrToStringUTF8((nint)pointer);
    }

    public nint GetExport(string name) => NativeLibrary.GetExport(_library, name);
}
