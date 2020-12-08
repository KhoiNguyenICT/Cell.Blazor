using System.Collections.Generic;

namespace Cell.Blazor.Data.Class
{
    public class SearchFilter
    {
        public List<string> Fields { get; set; }

        public string Key { get; set; }

        public string Operator { get; set; }

        public bool IgnoreCase { get; set; }
    }
}