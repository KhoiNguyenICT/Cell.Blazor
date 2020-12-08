using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Cell.Blazor._Core.Class
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class ListBoxLocale
    {
        [JsonPropertyName("noRecordsTemplate")]
        public string NoRecordsTemplate { get; set; } = "No records found";

        [JsonPropertyName("actionFailureTemplate")]
        public string ActionFailureTemplate { get; set; } = "Request failed";

        [JsonPropertyName("selectAllText")]
        public string SelectAllText { get; set; } = "Select All";

        [JsonPropertyName("unSelectAllText")]
        public string UnSelectAllText { get; set; } = "Unselect All";

        [JsonPropertyName("moveUp")]
        public string MoveUp { get; set; } = "Move Up";

        [JsonPropertyName("moveDown")]
        public string MoveDown { get; set; } = "Move Down";

        [JsonPropertyName("moveTo")]
        public string MoveTo { get; set; } = "Move To";

        [JsonPropertyName("moveFrom")]
        public string MoveFrom { get; set; } = "Move From";

        [JsonPropertyName("moveAllTo")]
        public string MoveAllTo { get; set; } = "Move All To";

        [JsonPropertyName("moveAllFrom")]
        public string MoveAllFrom { get; set; } = "Move All From";
    }
}