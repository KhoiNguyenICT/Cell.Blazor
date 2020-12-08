using Newtonsoft.Json;

namespace Cell.Blazor.Data.Class
{
    public class Aggregate
    {
        [JsonProperty("field")]
        public string Field { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}