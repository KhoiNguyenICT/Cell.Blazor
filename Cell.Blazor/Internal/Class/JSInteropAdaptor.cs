using Cell.Blazor.Internal.Interface;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;

namespace Cell.Blazor.Internal.Class
{
    public class JSInteropAdaptor : ComponentBase, IJSInteropAdaptor, IDisposable
    {
        public void Init() => this._dotnetRef = this.Create();

        private DotNetObjectReference<object> _dotnetRef { get; set; }

        public virtual DotNetObjectReference<object> Create() => DotNetObjectReference.Create<object>((object)this);

        public DotNetObjectReference<object> GetRef() => this._dotnetRef ?? this.Create();

        public void Dispose() => this.Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            this._dotnetRef?.Dispose();
        }
    }
}