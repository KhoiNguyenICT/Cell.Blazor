using System.ComponentModel;

namespace Cell.Blazor._Core.Class
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class ColorPickerLocale
    {
        public string Apply { get; set; } = nameof(Apply);

        public string Cancel { get; set; } = nameof(Cancel);

        public string ModeSwitcher { get; set; } = "Switch Mode";
    }
}