using System.Resources;

namespace Cell.Blazor._Core.Interface
{
    public interface ICellStringLocalizer
    {
        string GetText(string key);

        ResourceManager ResourceManager { get; }
    }
}