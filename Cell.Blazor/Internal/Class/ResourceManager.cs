using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Cell.Blazor._Core.Abstract;
using Cell.Blazor._Core.Interface;
using Cell.Blazor._Core.Service;
using Cell.Blazor.Internal.Static;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using Newtonsoft.Json;

namespace Cell.Blazor.Internal.Class
{
    public class ResourceManager : ComponentBase, IDisposable
    {
        private bool isFirstSource { get; set; }

        [CascadingParameter]
        public
#nullable disable
    BaseComponent Parent
        { get; set; }

        [Inject]
        public CellBlazorService SyncfusionService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        [JsonIgnore]
        public IHttpContextAccessor HttpContextAccessor { get; set; }

        [Inject]
        [JsonIgnore]
        public ICellStringLocalizer Localizer { get; set; }

        [Parameter]
        public List<string> LocaleKeys { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder) => base.BuildRenderTree(builder);

        private bool IsIE()
        {
            string str = "";
            HttpRequest request = HttpContextAccessor?.HttpContext?.Request;
            if (request != null)
                str = request.Headers["User-Agent"].ToString();
            return str.Contains("MSIE") || str.Contains("Trident");
        }

        private void SetCulture()
        {
            CultureInfo culture = CultureInfo.CurrentCulture;
            if (JsRuntime is IJSInProcessRuntime)
            {
                culture = CultureInfo.DefaultThreadCurrentCulture ?? Thread.CurrentThread.CurrentCulture;
                if (!(culture.Calendar is GregorianCalendar))
                    culture.DateTimeFormat = CultureInfo.InvariantCulture.DateTimeFormat;
            }
            Intl.SetCulture(culture);
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (!SyncfusionService.IsCultureLoaded)
            {
                SyncfusionService.IsCultureLoaded = true;
                SetCulture();
            }
            if (Parent != null)
            {
                using (LocalizerDetails localizerDetails = new LocalizerDetails(Localizer.ResourceManager, Intl.CurrentCulture, SyncfusionService, LocaleKeys))
                    Parent.LocaleText = localizerDetails.GetLocaleText();
            }
            if (SyncfusionService.JsRuntime != null)
                return;
            SyncfusionService.JsRuntime = JsRuntime;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender) => await base.OnAfterRenderAsync(firstRender);

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            Parent = null;
            LocaleKeys = null;
        }
    }
}