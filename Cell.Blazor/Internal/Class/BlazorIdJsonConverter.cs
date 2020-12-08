using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cell.Blazor.Internal.Class
{
    public class BlazorIdJsonConverter : JsonConverter
    {
        private readonly Type _types;

        public Dictionary<string, object> HashData { get; set; }

        public BlazorIdJsonConverter(Dictionary<string, object> Values)
        {
            if (Values.Count == 0)
                return;
            this._types = Values.FirstOrDefault<KeyValuePair<string, object>>().Value.GetType();
            this.HashData = Values;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken jtoken = JToken.FromObject(value);
            if (jtoken.GetType().Name == "JObject")
            {
                JObject jobject = (JObject)jtoken;
                string key = this.HashData.FirstOrDefault<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>)(x => x.Value == value)).Key;
                if (!jobject.ContainsKey("BlazId"))
                    jobject.AddFirst((object)new JProperty("BlazId", (object)key));
                jobject.WriteTo(writer);
            }
            else
                jtoken.WriteTo(writer);
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead => false;

        public override bool CanConvert(Type objectType) => this._types == objectType;
    }
}