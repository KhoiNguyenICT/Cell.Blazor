using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Cell.Blazor._Core.Class
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class FormValidatorLocale
    {
        [JsonPropertyName("required")]
        public string Required { get; set; } = "This field is required.";

        [JsonPropertyName("email")]
        public string Email { get; set; } = "Please enter a valid email address.";

        [JsonPropertyName("url")]
        public string Url { get; set; } = "Please enter a valid URL.";

        [JsonPropertyName("date")]
        public string Date { get; set; } = "Please enter a valid date.";

        [JsonPropertyName("dateIso")]
        public string DateIso { get; set; } = "Please enter a valid date ( ISO ).";

        [JsonPropertyName("creditcard")]
        public string Creditcard { get; set; } = "Please enter valid card number";

        [JsonPropertyName("number")]
        public string Number { get; set; } = "Please enter a valid number.";

        [JsonPropertyName("digits")]
        public string Digits { get; set; } = "Please enter only digits.";

        [JsonPropertyName("maxLength")]
        public string MaxLength { get; set; } = "Please enter no more than {0} characters.";

        [JsonPropertyName("minLength")]
        public string MinLength { get; set; } = "Please enter at least {0} characters.";

        [JsonPropertyName("rangeLength")]
        public string RangeLength { get; set; } = "Please enter a value between {0} and {1} characters long.";

        [JsonPropertyName("range")]
        public string Range { get; set; } = "Please enter a value between {0} and {1}.";

        [JsonPropertyName("max")]
        public string Max { get; set; } = "Please enter a value less than or equal to {0}.";

        [JsonPropertyName("min")]
        public string Min { get; set; } = "Please enter a value greater than or equal to {0}.";

        [JsonPropertyName("regex")]
        public string Regex { get; set; } = "Please enter a correct value.";

        [JsonPropertyName("tel")]
        public string Tel { get; set; } = "Please enter a valid phone number.";

        [JsonPropertyName("pattern")]
        public string Pattern { get; set; } = "Please enter a correct pattern value.";

        [JsonPropertyName("equalTo")]
        public string EqualTo { get; set; } = "Please enter the valid match text";
    }
}