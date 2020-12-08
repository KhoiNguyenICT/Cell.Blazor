using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Cell.Blazor._Core.Class
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class DropdownsLocale
    {
        [JsonPropertyName("noRecordsTemplate")]
        public string NoRecordsTemplate { get; set; } = "No records found";

        [JsonPropertyName("actionFailureTemplate")]
        public string ActionFailureTemplate { get; set; } = "Request failed";

        [JsonPropertyName("overflowCountTemplate")]
        public string OverflowCountTemplate { get; set; } = "+${count} more..";

        [JsonPropertyName("selectAllText")]
        public string SelectAllText { get; set; } = "Select All";

        [JsonPropertyName("unSelectAllText")]
        public string UnSelectAllText { get; set; } = "Unselect All";

        [JsonPropertyName("totalCountTemplate")]
        public string TotalCountTemplate { get; set; } = "${count} selected";
    }
}