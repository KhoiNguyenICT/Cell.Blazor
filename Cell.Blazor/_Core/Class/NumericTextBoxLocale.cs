using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Cell.Blazor._Core.Class
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class NumericTextBoxLocale
    {
        [JsonPropertyName("incrementTitle")]
        public string IncrementTitle { get; set; } = "Increment value";

        [JsonPropertyName("decrementTitle")]
        public string DecrementTitle { get; set; } = "Decrement value";
    }
}