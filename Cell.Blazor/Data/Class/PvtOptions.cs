using System.Collections.Generic;

namespace Cell.Blazor.Data.Class
{
    public class PvtOptions
    {
        public object Groups { get; set; }

        public List<Aggregate> Aggregates { get; set; }

        public object Search { get; set; }

        public int ChangeSet { get; set; }

        public List<object> Searches { get; set; }

        public int position { get; set; }
    }
}