using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Cell.Blazor._Core.Class
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class AutoCompleteLocale
    {
        [JsonPropertyName("noRecordsTemplate")]
        public string NoRecordsTemplate { get; set; } = "No records found";

        [JsonPropertyName("actionFailureTemplate")]
        public string ActionFailureTemplate { get; set; } = "Request failed";
    }
}