using Microsoft.AspNetCore.Http;

namespace Cell.Blazor._Core.Interface
{
    public interface ICellBlazor
    {
        HttpContext GetContext();
    }
}