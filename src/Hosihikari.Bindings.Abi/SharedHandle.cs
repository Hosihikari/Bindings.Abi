namespace Hosihikari.Bindings.Abi;

/// <summary>
/// Keeps one shared reference alive. <see cref="Target"/> is a borrowed address;
/// disposing this handle returns the reference to the native owner set.
/// </summary>
public sealed unsafe class SharedHandle : IDisposable
{
    private nint _holder;
    private delegate* unmanaged[Cdecl]<void*, void> _release;

    private SharedHandle(nint holder, nint target, delegate* unmanaged[Cdecl]<void*, void> release)
    {
        _holder = holder;
        _release = release;
        Target = target;
    }

    public static SharedHandle? FromNative(SharedHandleData data) =>
        data.Holder is null
            ? null
            : new SharedHandle((nint)data.Holder, (nint)data.Target, data.Release);

    public nint Target { get; }

    public nint Handle => Target;

    public bool IsDisposed => _holder == 0;

    /// <summary>Returns the raw handle shape so the shim can borrow the shared reference.</summary>
    public SharedHandleData ToNative() => new()
    {
        Holder = (void*)_holder,
        Target = (void*)Target,
        Release = _release,
    };

    public void Dispose()
    {
        if (_holder != 0)
        {
            _release((void*)_holder);
            _holder = 0;
        }
    }
}
