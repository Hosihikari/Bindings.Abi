# Hosihikari.Bindings.Abi

The shared ABI layer for the Hosihikari bindings: value types used by every BindingShim and the managed loader that validates a versioned function table.

## Contents

- `Hosihikari.Bindings.Abi.StringView` — borrowed UTF-8 buffer.
- `Hosihikari.Bindings.Abi.ArrayView` — borrowed element array.
- `Hosihikari.Bindings.Abi.SharedHandleData` / `SharedHandle` — `std::shared_ptr` projection and its releasable handle. `ToNative()` returns the raw shape so generated shims can pass the shared reference back as a `shared_ptr` or `weak_ptr` argument. Disposed handles are recycled from a per-thread pool, so a handle must not be used after `Dispose`.
- `Hosihikari.Bindings.Abi.ShimLibrary` — loads a shim, checks the table version and size, keeps the native library alive, and exposes `GetLastError()` and `GetExport()`.
- `native/hosihikari_abi.h` — the matching C++ layout, export macro, and thread-local error boundary. It is also shipped inside the NuGet package.

## Layout contract

The managed structs and the C++ structs are checked independently only by size/version at load time. Any change to the field order, field types, or the table header requires a table version bump:

| Native | Managed |
|---|---|
| `HosihikariStringView` | `StringView` |
| `HosihikariArrayView` | `ArrayView` |
| `HosihikariSharedHandle` | `SharedHandleData` |
| `HosihikariMapEntry` | `MapEntry` |

The table itself stays surface-specific: each Interop assembly generates its own `NativeMethods.Table` and passes `sizeof(Table)` to `ShimLibrary.Load`.

## Generate

BindingShims include `hosihikari_abi.h` from their generated shim directory. `Hosihikari/Bindings.Generator` copies this header into each emitted shim when invoked with `--abi-header native/hosihikari_abi.h`.

## Build

```shell
dotnet build Hosihikari.Bindings.Abi.slnx -c Release
dotnet pack src/Hosihikari.Bindings.Abi/Hosihikari.Bindings.Abi.csproj -c Release
```

## Licence

LGPL-3.0-or-later, see `LICENSE.md`.
