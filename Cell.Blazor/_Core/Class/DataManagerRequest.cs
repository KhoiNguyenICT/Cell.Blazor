using System.Collections.Generic;
using Cell.Blazor.Data.Class;
using Newtonsoft.Json;

namespace Cell.Blazor._Core.Class
{
    public class DataManagerRequest
    {
        [JsonProperty("skip")]
        public int Skip { get; set; }

        [JsonProperty("take")]
        public int Take { get; set; }

        [JsonProperty("antiForgery")]
        public string antiForgery { get; set; }

        [JsonProperty("requiresCounts")]
        public bool RequiresCounts { get; set; }

        [JsonProperty("table")]
        public string Table { get; set; }

        [JsonProperty("IdMapping")]
        public string IdMapping { get; set; }

        [JsonProperty("group")]
        public List<string> Group { get; set; }

        [JsonProperty("select")]
        public List<string> Select { get; set; }

        [JsonProperty("expand")]
        public List<string> Expand { get; set; }

        [JsonProperty("sorted")]
        public List<Sort> Sorted { get; set; }

        [JsonProperty("search")]
        public List<SearchFilter> Search { get; set; }

        [JsonProperty("where")]
        public List<WhereFilter> Where { get; set; }

        [JsonProperty("aggregates")]
        public List<Aggregate> Aggregates { get; set; }

        [JsonProperty("params")]
        public IDictionary<string, object> Params { get; set; }

        [JsonProperty("distinct")]
        public List<string> Distinct { get; set; }

        public IDictionary<string, string> GroupByFormatter { get; set; }

        public bool ServerSideGroup { get; set; } = true;
    }
}