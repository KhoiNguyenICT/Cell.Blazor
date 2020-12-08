using Cell.Blazor._Core.Static;
using Cell.Blazor.Internal.Static;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Cell.Blazor._Core.Class
{
    public class Base
    {
        public Base(IJSRuntime jsRuntime) => JsRunTime = jsRuntime;

        public IJSRuntime JsRunTime { get; set; }

        [Obsolete("This EnableRipple method is deprecated. Use the SyncfusionBlazorService.EnableRipple method to achieve ripple effect to the Syncfusion Blazor component.")]
        public Base EnableRipple(bool isRipple)
        {
            CellBase.EnableRipple = isRipple;
            JsRunTime.InvokeAsync<bool>("sfBlazor.enableRipple", (object)isRipple);
            return this;
        }

        [Obsolete("This EnableRtl method is deprecated. Use the SyncfusionBlazorService.EnableRtl method to achieve Rtl text direction to the Syncfusion Blazor component.")]
        public Base EnableRtl(bool isRtl)
        {
            CellBase.EnableRtl = isRtl;
            JsRunTime.InvokeAsync<bool>("sfBlazor.enableRtl", (object)isRtl);
            return this;
        }

        [Obsolete("This IsDevice method is deprecated. Use the SyncfusionBlazorService.IsDevice method to ensure whether the app is running in the device or not.")]
        public async Task<bool> IsDevice() => await JsRunTime.InvokeAsync<bool>("sfBlazor.isDevice");

        [Obsolete("This SetCulture method is deprecated. Syncfusion Blazor UI Components are now using the resource files for Localization. For further details refer to the following UG documentation. https://blazor.syncfusion.com/documentation/common/localization/")]
        public Base SetCulture(string cultureName)
        {
            if (string.IsNullOrEmpty(cultureName))
                cultureName = "en-US";
            Intl.SetCulture(cultureName);
            JsRunTime.InvokeAsync<object>("sfBlazor.loadCldr", (object)GlobalizeJsonGenerator.GetGlobalizeJsonString(Intl.CurrentCulture));
            JsRunTime.InvokeAsync<string>("sfBlazor.setCulture", (object)cultureName, Intl.GetCultureFormats(cultureName));
            return this;
        }

        [Obsolete("This SetCurrencyCode method is deprecated. Syncfusion Blazor UI Components are now using the resource files for Localization. For further details refer to the following UG documentation. https://blazor.syncfusion.com/documentation/common/localization/")]
        public Base SetCurrencyCode(string currencyCode)
        {
            JsRunTime.InvokeAsync<string>("sfBlazor.setCurrencyCode", (object)currencyCode);
            return this;
        }

        [Obsolete("This LoadCldrData method is deprecated. Syncfusion Blazor UI Components are now using the resource files for Localization. For further details refer to the following UG documentation. https://blazor.syncfusion.com/documentation/common/localization/")]
        public Base LoadCldrData(params string[] cldrData)
        {
            string[] strArray = new string[cldrData.Length];
            for (int index = 0; index < cldrData.Length; ++index)
                strArray[index] = ReadFiles(cldrData[index]);
            JsRunTime.InvokeAsync<object>("sfBlazor.loadCldr", strArray);
            return this;
        }

        [Obsolete("This LoadCldrData method is deprecated. Syncfusion Blazor UI Components are now using the resource files for Localization. For further details refer to the following UG documentation. https://blazor.syncfusion.com/documentation/common/localization/")]
        public Base LoadCldrData(params object[] cldrData)
        {
            string[] strArray = new string[cldrData.Length];
            for (int index = 0; index < cldrData.Length; ++index)
                strArray[index] = JsonConvert.SerializeObject(cldrData[index]);
            JsRunTime.InvokeAsync<object>("sfBlazor.loadCldr", strArray);
            return this;
        }

        [Obsolete("This LoadLocaleData method is deprecated. Syncfusion Blazor UI Components are now using the resource files for Localization. For further details refer to the following UG documentation. https://blazor.syncfusion.com/documentation/common/localization/")]
        public Base LoadLocaleData(string localePath)
        {
            JsRunTime.InvokeAsync<object>("sfBlazor.load", (object)ReadFiles(localePath));
            return this;
        }

        [Obsolete("This LoadLocaleData method is deprecated. Syncfusion Blazor UI Components are now using the resource files for Localization. For further details refer to the following UG documentation. https://blazor.syncfusion.com/documentation/common/localization/")]
        public Base LoadLocaleData(object localeObject)
        {
            JsRunTime.InvokeAsync<object>("sfBlazor.load", (object)JsonSerializer.Serialize(localeObject));
            return this;
        }

        public string ReadFiles(string path) => JsonConvert.SerializeObject(JsonConvert.DeserializeObject<object>(File.ReadAllText(Path.GetFullPath(path))), Formatting.Indented, new JsonSerializerSettings
        {
            StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
        });
    }
}