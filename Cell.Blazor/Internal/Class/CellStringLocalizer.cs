using Cell.Blazor._Core.Interface;
using System.Globalization;

namespace Cell.Blazor.Internal.Class
{
    public class CellStringLocalizer : ICellStringLocalizer
    {
        public string GetText(string key) => this.ResourceManager?.GetString(key, CultureInfo.CurrentCulture);

        public System.Resources.ResourceManager ResourceManager => null;
    }
}