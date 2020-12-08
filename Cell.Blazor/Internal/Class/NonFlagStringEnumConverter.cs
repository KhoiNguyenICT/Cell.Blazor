using System;
using Newtonsoft.Json.Converters;

namespace Cell.Blazor.Internal.Class
{
    public class NonFlagStringEnumConverter : StringEnumConverter
    {
        public override bool CanConvert(Type objectType)
        {
            if (!base.CanConvert(objectType))
                return false;
            Type type = Nullable.GetUnderlyingType(objectType);
            if ((object)type == null)
                type = objectType;
            return type.GetCustomAttributes(typeof(FlagsAttribute), false).Length == 0;
        }
    }
}