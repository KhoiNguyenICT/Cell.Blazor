using System;
using System.Collections.Generic;
using System.Reflection;
using Cell.Blazor._Core.Interface;
using Cell.Blazor._Core.Static;

namespace Cell.Blazor._Core.Class
{
    public class ModelMetadataProvider : IMetadataProvider
    {
        public Metadata Read(PropertyInfo property) => property.GetMetadata();

        public Dictionary<string, Metadata> ReadFromType(Type modelType)
        {
            PropertyInfo[] properties = modelType.GetProperties();
            Dictionary<string, Metadata> dictionary = new Dictionary<string, Metadata>();
            foreach (PropertyInfo property in properties)
                dictionary.TryAdd(property.Name, property.GetMetadata());
            return dictionary;
        }

        public Metadata FromType(Type modelType, string propertyName)
        {
            PropertyInfo property = modelType.GetProperty(propertyName);
            return !(property != null) ? null : property.GetMetadata();
        }
    }
}