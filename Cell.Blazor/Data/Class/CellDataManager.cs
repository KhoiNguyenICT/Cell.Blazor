using Cell.Blazor._Core.Class;
using Cell.Blazor._Core.Static;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Cell.Blazor.Data.Class
{
    public class CellDataManager : DataManager
    {
        protected override void BuildRenderTree(RenderTreeBuilder __builder) => TypeInference.CreateCascadingValue<CellDataManager>(__builder, 0, 1, this, 2, (RenderFragment)(__builder2 => __builder2.AddContent(3, this.ChildContent)));
    }
}