using System.Reflection;
using Cell.Blazor._Core.Class;

namespace Cell.Blazor._Core.Static
{
    public static class MetadataExtension
    {
        public static void FromPropertyInfo(this Metadata meta, PropertyInfo property)
        {
            Annotation annotation = new Annotation(property, meta);
        }

        public static Metadata GetMetadata(this PropertyInfo property)
        {
            Metadata meta = new Metadata();
            Annotation annotation = new Annotation(property, meta);
            return meta;
        }
    }
}