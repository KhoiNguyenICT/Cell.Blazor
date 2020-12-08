using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;

namespace Cell.Blazor._Core.Static
{
    public class TypeInference
    {
        public static void CreateCascadingValue<TValue>(
            RenderTreeBuilder __builder,
            int seq,
            int __seq0,
            TValue __arg0,
            int __seq1,
            RenderFragment __arg1)
        {
            __builder.OpenComponent<CascadingValue<TValue>>(seq);
            __builder.AddAttribute(__seq0, "Value", (object)__arg0);
            __builder.AddAttribute(__seq1, "ChildContent", (MulticastDelegate)__arg1);
            __builder.CloseComponent();
        }
    }
}