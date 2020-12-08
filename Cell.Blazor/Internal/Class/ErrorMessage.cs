using Newtonsoft.Json;

namespace Cell.Blazor.Internal.Class
{
    public class ErrorMessage
    {
        [JsonProperty("message")]
        internal string Message { get; set; }

        [JsonProperty("stack")]
        internal string Stack { get; set; }
    }
}