using System;
using Microsoft.JSInterop;

namespace Cell.Blazor.Internal.Interface
{
    public interface IJSInteropAdaptor : IDisposable
    {
        void Init();

        DotNetObjectReference<object> Create();

        DotNetObjectReference<object> GetRef();
    }
}