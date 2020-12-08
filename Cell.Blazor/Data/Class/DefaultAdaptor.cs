using System;
using System.ComponentModel;
using Cell.Blazor._Core.Class;
using Cell.Blazor._Core.Enums;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Cell.Blazor.Data.Class
{
    public class DefaultAdaptor : IDisposable
    {
        [Parameter]
        [JsonProperty("adaptor")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Adaptors Adaptor { get; set; } = Adaptors.BlazorAdaptor;

        [JsonProperty("adaptorName")]
        public Adaptors AdaptorName { get; set; }

        [JsonProperty("key")]
        internal string Key { get; set; }

        [JsonProperty("url")]
        [DefaultValue("")]
        public string Url { get; set; } = string.Empty;

        [JsonProperty("offline")]
        public bool Offline { get; set; }

        public DefaultAdaptor()
        {
        }

        public DefaultAdaptor(string key, Adaptors adaptorName = Adaptors.BlazorAdaptor)
        {
            Key = key;
            AdaptorName = adaptorName;
        }

        public DefaultAdaptor(string key, DataManager manager, Adaptors adaptorName = Adaptors.BlazorAdaptor)
        {
            Key = key;
            AdaptorName = adaptorName;
            Url = manager?.Url;
            Offline = manager != null && manager.Offline;
        }

        public void Dispose()
        {
        }
    }
}