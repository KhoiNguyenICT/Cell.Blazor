using Cell.Blazor._Core.Abstract;
using Cell.Blazor._Core.Interface;
using Cell.Blazor._Core.Static;
using Cell.Blazor.Internal.Class;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Cell.Blazor._Core.Service
{
    public class CellBlazorService : ICellBlazor
    {
        private IHttpContextAccessor _contextAccessor;

        public IJSRuntime JsRuntime { get; set; }

        public bool IsCultureLoaded { get; set; }

        public bool? IsPrerendering { get; set; }

        public bool IsLicenseClosed { get; set; }

        public bool IsScriptRendered { get; set; }

        public bool IsLicenseRendered { get; set; }

        public bool IsRippleEnabled { get; set; }

        public bool IsRtlEnabled { get; set; }

        public bool IsDeviceMode { get; set; }

        public string ScriptHashKey { get; set; } = "06b4eb";

        public bool ICellirstLicenseComponent { get; set; }

        public bool ICellirstResource { get; set; } = true;

        public bool ICellirstBaseResource { get; set; } = true;

        public bool IsDeviceInvoked { get; set; }

        public Dictionary<string, List<string>> LoadedLocale { get; set; } = new Dictionary<string, List<string>>();

        public List<BaseComponent> ComponentQueue { get; set; } = new List<BaseComponent>();

        public void EnableRipple()
        {
            CellBase.EnableRipple = true;
            this.IsRippleEnabled = true;
        }

        public void EnableRtl(bool enable = true)
        {
            CellBase.EnableRtl = enable;
            this.IsRtlEnabled = enable;
        }

        public async ValueTask<bool> IsDevice()
        {
            CellBlazorService CellService = this;
            if (CellService.JsRuntime != null)
                await CellBaseUtils.InvokeDeviceMode(CellService, CellService.JsRuntime);
            return CellService.IsDeviceMode;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public CellBlazorService(IHttpContextAccessor httpContextAccessor)
        {
            this._contextAccessor = httpContextAccessor;
            IHttpContextAccessor contextAccessor = this._contextAccessor;
            bool? nullable;
            if (contextAccessor == null)
            {
                nullable = new bool?();
            }
            else
            {
                HttpContext httpContext = contextAccessor.HttpContext;
                if (httpContext == null)
                {
                    nullable = new bool?();
                }
                else
                {
                    HttpResponse response = httpContext.Response;
                    nullable = response != null ? new bool?(!response.HasStarted) : new bool?();
                }
            }
            this.IsPrerendering = nullable;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public HttpContext GetContext() => this._contextAccessor?.HttpContext?.Request?.HttpContext;
    }
}