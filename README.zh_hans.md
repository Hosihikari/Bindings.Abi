# Hosihikari.Bindings.Abi

Hosihikari 绑定的共享 ABI 层：各 BindingShim 共用的值类型，以及校验版本化函数表的托管加载器。

## 内容

- `Hosihikari.Bindings.Abi.StringView` — 借用的 UTF-8 缓冲区。
- `Hosihikari.Bindings.Abi.ArrayView` — 借用的元素数组。
- `Hosihikari.Bindings.Abi.SharedHandleData` / `SharedHandle` — `std::shared_ptr` 投影及可释放句柄。
- `Hosihikari.Bindings.Abi.ShimLibrary` — 加载 shim、校验表版本与大小、保持原生库句柄，并暴露 `GetLastError()` 与 `GetExport()`。
- `native/hosihikari_abi.h` — 对应的 C++ 布局、导出宏与线程局部错误边界，同时随 NuGet 包发布。

## 布局契约

托管结构体与 C++ 结构体仅通过加载时的版本/大小检查间接校验。字段顺序、字段类型或表头变化必须提升表版本。

| 原生 | 托管 |
|---|---|
| `HosihikariStringView` | `StringView` |
| `HosihikariArrayView` | `ArrayView` |
| `HosihikariSharedHandle` | `SharedHandleData` |
| `HosihikariMapEntry` | `MapEntry` |

函数表本身仍按 surface 生成：每个 Interop 程序集拥有自己的 `NativeMethods.Table`，并把 `sizeof(Table)` 传给 `ShimLibrary.Load`。

## 生成

BindingShim 从生成目录包含 `hosihikari_abi.h`。`Hosihikari/Bindings.Generator` 在带 `--abi-header native/hosihikari_abi.h` 发射时把该头文件复制到每个 shim 目录。

## 构建

```shell
dotnet build Hosihikari.Bindings.Abi.slnx -c Release
dotnet pack src/Hosihikari.Bindings.Abi/Hosihikari.Bindings.Abi.csproj -c Release
```

## 许可

LGPL-3.0-or-later，见 `LICENSE.md`。
