using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Cell.Blazor._Core.Class
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class SliderLocale
    {
        [JsonPropertyName("incrementTitle")]
        public string IncrementTitle { get; set; } = "Increase";

        [JsonPropertyName("decrementTitle")]
        public string DecrementTitle { get; set; } = "Decrease";
    }
}