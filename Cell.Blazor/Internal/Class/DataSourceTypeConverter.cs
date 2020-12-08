using Cell.Blazor._Core.Class;
using Newtonsoft.Json;
using System;

namespace Cell.Blazor.Internal.Class
{
    internal class DataSourceTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(string);

        public override bool CanRead => false;

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value.GetType() == typeof(string))
                writer.WriteRawValue((string)value);
            else if (value.GetType() == typeof(DataManager))
            {
                string str = JsonConvert.SerializeObject(value, Formatting.Indented);
                writer.WriteRawValue("new sf.data.DataManager(" + str + ")");
            }
            else
                writer.WriteRawValue(JsonConvert.SerializeObject(value, Formatting.Indented));
        }
    }
}