using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Cell.Blazor._Core.Class
{
    public class LocaleData
    {
        [JsonPropertyName("diagram")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public DiagramLocale Diagram { get; set; } = new DiagramLocale();

        [JsonPropertyName("dropdowns")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public DropdownsLocale Dropdowns { get; set; } = new DropdownsLocale();

        [JsonPropertyName("drop-down-list")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public DropDownListLocale DropDownList { get; set; } = new DropDownListLocale();

        [JsonPropertyName("combo-box")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ComboBoxLocale ComboBox { get; set; } = new ComboBoxLocale();

        [JsonPropertyName("auto-complete")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public AutoCompleteLocale AutoComplete { get; set; } = new AutoCompleteLocale();

        [JsonPropertyName("multi-select")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public MultiSelectLocale MultiSelect { get; set; } = new MultiSelectLocale();

        [JsonPropertyName("listbox")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ListBoxLocale ListBox { get; set; } = new ListBoxLocale();

        [JsonPropertyName("colorpicker")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ColorPickerLocale ColorPicker { get; set; } = new ColorPickerLocale();

        [JsonPropertyName("uploader")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public UploaderLocale Uploader { get; set; } = new UploaderLocale();

        [JsonPropertyName("numerictextbox")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public NumericTextBoxLocale NumericTextBox { get; set; } = new NumericTextBoxLocale();

        [JsonPropertyName("slider")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public SliderLocale Slider { get; set; } = new SliderLocale();

        [JsonPropertyName("formValidator")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public FormValidatorLocale FormValidator { get; set; } = new FormValidatorLocale();
    }
}