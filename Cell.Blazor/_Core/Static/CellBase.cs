using Cell.Blazor._Core.Class;
using Cell.Blazor.Internal.Class;
using Cell.Blazor.Internal.Static;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Cell.Blazor._Core.Static
{
    public static class CellBase
    {
        public static bool IsLicenseRendered { get; set; }

        public static string LicenseContent { get; set; }

        public static bool DisableScriptManager { get; set; }

        public static List<string> LoadedScripts { get; set; } = new List<string>();

        [Obsolete("This Sf extension is deprecated. Use SyncfusionBlazorService to perform equivalent functionalities.")]
        public static Base Sf(this IJSRuntime jsRuntime) => new Base(jsRuntime);

        public static string GetEnumValue<T>(T value) where T : struct, IConvertible
        {
            MemberInfo element = typeof(T).GetTypeInfo().DeclaredMembers.SingleOrDefault<MemberInfo>((Func<MemberInfo, bool>)(x => x.Name == value.ToString()));
            if ((object)element == null)
                return (string)null;
            return element.GetCustomAttribute<EnumMemberAttribute>(false)?.Value;
        }

        public static string GetEnumValue<T>(T? value) where T : struct, IConvertible
        {
            MemberInfo element = typeof(T).GetTypeInfo().DeclaredMembers.SingleOrDefault<MemberInfo>((Func<MemberInfo, bool>)(x => x.Name == value.ToString()));
            if ((object)element == null)
                return (string)null;
            return element.GetCustomAttribute<EnumMemberAttribute>(false)?.Value;
        }

        public static bool Equals<T>(T oldObject, T newObject)
        {
            if ((object)newObject == null)
                return false;
            string b = JsonConvert.SerializeObject((object)newObject);
            return string.Equals(JsonConvert.SerializeObject((object)oldObject), b);
        }

        public static async Task Animate(
          IJSRuntime jsRuntime,
          ElementReference reference,
          AnimationSettings animationObject)
        {
            await CellInterop.Animate(jsRuntime, (object)reference, (object)animationObject);
        }

        public static async Task RippleEffect(
          IJSRuntime jsRuntime,
          ElementReference reference,
          RippleSettings rippleObject)
        {
            await CellInterop.RippleEffect(jsRuntime, (object)reference, (object)rippleObject);
        }

        public static bool EnableRipple { get; set; }

        public static bool EnableRtl { get; set; }
    }
}