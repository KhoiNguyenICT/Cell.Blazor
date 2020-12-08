using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Cell.Blazor._Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AnimationEffect
    {
        [EnumMember(Value = "SlideLeftIn")] SlideLeftIn,
        [EnumMember(Value = "SlideRightIn")] SlideRightIn,
        [EnumMember(Value = "FadeIn")] FadeIn,
        [EnumMember(Value = "FadeOut")] FadeOut,
        [EnumMember(Value = "FadeZoomIn")] FadeZoomIn,
        [EnumMember(Value = "FadeZoomOut")] FadeZoomOut,
        [EnumMember(Value = "ZoomIn")] ZoomIn,
        [EnumMember(Value = "ZoomOut")] ZoomOut,
        [EnumMember(Value = "SlideLeft")] SlideLeft,
        [EnumMember(Value = "SlideRight")] SlideRight,
        [EnumMember(Value = "FlipLeftDownIn")] FlipLeftDownIn,
        [EnumMember(Value = "FlipLeftDownOut")] FlipLeftDownOut,
        [EnumMember(Value = "FlipLeftUpIn")] FlipLeftUpIn,
        [EnumMember(Value = "FlipLeftUpOut")] FlipLeftUpOut,
        [EnumMember(Value = "FlipRightDownIn")] FlipRightDownIn,
        [EnumMember(Value = "FlipRightDownOut")] FlipRightDownOut,
        [EnumMember(Value = "FlipRightUpIn")] FlipRightUpIn,
        [EnumMember(Value = "FlipRightUpOut")] FlipRightUpOut,
        [EnumMember(Value = "FlipXDownIn")] FlipXDownIn,
        [EnumMember(Value = "FlipXDownOut")] FlipXDownOut,
        [EnumMember(Value = "FlipXUpIn")] FlipXUpIn,
        [EnumMember(Value = "FlipXUpOut")] FlipXUpOut,
        [EnumMember(Value = "FlipYLeftIn")] FlipYLeftIn,
        [EnumMember(Value = "FlipYLeftOut")] FlipYLeftOut,
        [EnumMember(Value = "FlipYRightIn")] FlipYRightIn,
        [EnumMember(Value = "FlipYRightOut")] FlipYRightOut,
        [EnumMember(Value = "SlideBottomIn")] SlideBottomIn,
        [EnumMember(Value = "SlideBottomOut")] SlideBottomOut,
        [EnumMember(Value = "SlideDown")] SlideDown,
        [EnumMember(Value = "SlideUp")] SlideUp,
        [EnumMember(Value = "SlideLeftOut")] SlideLeftOut,
        [EnumMember(Value = "SlideRightOut")] SlideRightOut,
        [EnumMember(Value = "SlideTopIn")] SlideTopIn,
        [EnumMember(Value = "SlideTopOut")] SlideTopOut,
        [EnumMember(Value = "None")] None,
    }
}