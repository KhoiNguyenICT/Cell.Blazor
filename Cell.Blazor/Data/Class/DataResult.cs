using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

namespace Cell.Blazor.Data.Class
{
    public class DataResult : DataResult<object>
    {
    }

    public class DataResult<T>
    {
        [JsonProperty("result")]
        public IEnumerable Result { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("aggregates")]
        public IDictionary<string, object> Aggregates { get; set; }
    }
}