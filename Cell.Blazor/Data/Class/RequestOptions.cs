using System;
using System.Net.Http;

namespace Cell.Blazor.Data.Class
{
    public class RequestOptions
    {
        public string Url { get; set; }

        public string BaseUrl { get; set; }

        public HttpMethod RequestMethod { get; set; }

        public object Data { get; set; }

        public Query Queries { get; set; }

        public string ContentType { get; set; } = "application/json";

        internal PvtOptions PvtData { get; set; }

        internal object Original { get; set; }

        internal Func<object[], int, string, string> UrlFunc { get; set; }

        internal Func<object[], int, string> DataFunc { get; set; }

        internal string CSet { get; set; }

        internal string keyField { get; set; }

        internal string Accept { get; set; }

        internal HttpMethod UpdateType { get; set; }
    }
}