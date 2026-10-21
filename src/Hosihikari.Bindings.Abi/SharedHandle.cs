namespace Hosihikari.Bindings.Abi;

/// <summary>
/// Keeps one shared reference alive. <see cref="Target"/> is a borrowed address;
/// disposing this handle returns the reference to the native owner set.
/// Disposed instances are recycled, so a handle must not be used after <see cref="Dispose"/>.
/// </summary>
public sealed unsafe class SharedHandle : IDisposable
{
    private const int MaxPooledPerThread = 64;

    [ThreadStatic]
    private static SharedHandle? tPool;

    [ThreadStatic]
    private static int tPoolCount;

    private nint _holder;
    private nint _target;
    private delegate* unmanaged[Cdecl]<void*, void> _release;
    private SharedHandle? _next;

    private SharedHandle()
    {
    }

    public static SharedHandle? FromNative(SharedHandleData data)
    {
        if (data.Holder is null)
        {
            return null;
        }

        var handle = Rent();
        handle._holder = (nint)data.Holder;
        handle._target = (nint)data.Target;
        handle._release = data.Release;
        return handle;
    }

    public nint Target => _target;

    public nint Handle => _target;

    public bool IsDisposed => _holder == 0;

    /// <summary>Returns the raw handle shape so the shim can borrow the shared reference.</summary>
    public SharedHandleData ToNative() => new()
    {
        Holder = (void*)_holder,
        Target = (void*)_target,
        Release = _release,
    };

    public void Dispose()
    {
        if (_holder != 0)
        {
            var holder = _holder;
            var release = _release;
            _holder = 0;
            _target = 0;
            _release = null;
            release((void*)holder);
            ReturnToPool();
        }
    }

    private static SharedHandle Rent()
    {
        var pooled = tPool;
        if (pooled is null)
        {
            return new SharedHandle();
        }

        tPool = pooled._next;
        pooled._next = null;
        tPoolCount--;
        return pooled;
    }

    private void ReturnToPool()
    {
        if (tPoolCount >= MaxPooledPerThread)
        {
            return;
        }

        _next = tPool;
        tPool = this;
        tPoolCount++;
    }
}
