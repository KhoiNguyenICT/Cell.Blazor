using System.Runtime.Serialization;

namespace Cell.Blazor._Core.Enums
{
    public enum Adaptors
    {
        [EnumMember(Value = "JsonAdaptor")] JsonAdaptor,
        [EnumMember(Value = "BlazorAdaptor")] BlazorAdaptor,
        [EnumMember(Value = "ODataAdaptor")] ODataAdaptor,
        [EnumMember(Value = "ODataV4Adaptor")] ODataV4Adaptor,
        [EnumMember(Value = "UrlAdaptor")] UrlAdaptor,
        [EnumMember(Value = "WebApiAdaptor")] WebApiAdaptor,
        [EnumMember(Value = "RemoteSaveAdaptor")] RemoteSaveAdaptor,
        [EnumMember(Value = "CustomAdaptor")] CustomAdaptor,
    }
}