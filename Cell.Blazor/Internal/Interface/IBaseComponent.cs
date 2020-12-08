using Cell.Blazor._Core.Class;
using System;
using System.Collections.Generic;

namespace Cell.Blazor.Internal.Interface
{
    public interface IBaseComponent
    {
        bool IsRendered { get; set; }

        bool TemplateClientChanges { get; set; }

        Type ModelType { get; set; }

        Dictionary<string, object> DataContainer { get; set; }

        Dictionary<string, object> DataHashTable { get; set; }

        DataManager DataManager { get; set; }

        string GetJSNamespace();
    }
}