using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Cell.Blazor._Core.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Operator
    {
        [EnumMember(Value = "none")] None,
        [EnumMember(Value = "contains")] Contains,
        [EnumMember(Value = "startswith")] StartsWith,
        [EnumMember(Value = "endswith")] EndsWith,
        [EnumMember(Value = "equal")] Equal,
        [EnumMember(Value = "notequal")] NotEqual,
        [EnumMember(Value = "greaterthan")] GreaterThan,
        [EnumMember(Value = "greaterthanorequal")] GreaterThanOrEqual,
        [EnumMember(Value = "lessthan")] LessThan,
        [EnumMember(Value = "lessthanorequal")] LessThanOrEqual,
    }
}