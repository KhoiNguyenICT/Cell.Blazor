using System;
using System.Collections.Generic;
using System.Reflection;
using Cell.Blazor._Core.Class;

namespace Cell.Blazor._Core.Interface
{
    public interface IMetadataProvider
    {
        Metadata Read(PropertyInfo property);

        Dictionary<string, Metadata> ReadFromType(Type modelType);

        Metadata FromType(Type modelType, string propertyName);
    }
}