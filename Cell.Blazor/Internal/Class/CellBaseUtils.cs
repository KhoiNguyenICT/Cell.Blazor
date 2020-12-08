using Cell.Blazor._Core.Abstract;
using Cell.Blazor._Core.Service;
using Cell.Blazor._Core.Static;
using Cell.Blazor.Internal.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Cell.Blazor.Internal.Class
{
    public class CellBaseUtils
    {
        private const string IMPORT = "import";

        private const string SCRIPT_PATH = "./_content/Cell.Blazor/scripts/";

        public static Dictionary<string, object> UpdateDictionary(
          string key,
          object data,
          Dictionary<string, object> dictionary)
        {
            if (!dictionary.TryAdd(key, data))
                dictionary[key] = data;
            return dictionary;
        }

        public static Dictionary<string, object> GetAttribtues(
          string classList,
          Dictionary<string, object> dictionary)
        {
            if (!dictionary.TryAdd("class", (object)classList))
                dictionary["class"] = (object)CellBaseUtils.AddClass(classList, dictionary["class"].ToString());
            return dictionary;
        }

        public static bool Equals<T>(T oldValue, T newValue)
        {
            Type type = oldValue?.GetType();
            if ((!(type != (Type)null) ? 0 : (type.Namespace == null || !type.Namespace.Contains("Collections") ? (type.IsArray ? 1 : 0) : 1)) == 0)
                return EqualityComparer<T>.Default.Equals(oldValue, newValue);
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };
            return string.Equals(System.Text.Json.JsonSerializer.Serialize<T>(oldValue, options), System.Text.Json.JsonSerializer.Serialize<T>(newValue, options));
        }

        public static async Task<T> UpdateProperty<T>(
          T publicValue,
          T privateValue,
          EventCallback<T> eventCallback,
          EditContext editContext = null,
          Expression<Func<T>> expression = null)
        {
            T finalValue = publicValue;
            if (eventCallback.HasDelegate && !CellBaseUtils.Equals<T>(publicValue, privateValue))
            {
                await eventCallback.InvokeAsync(finalValue);
                if (editContext != null)
                    CellBaseUtils.ValidateExpression<T>(editContext, expression);
            }
            T obj = finalValue;
            finalValue = default(T);
            return obj;
        }

        public static double[] ToDoubleArray(object args)
        {
            List<double> doubleList = new List<double>();
            foreach (object obj in args as IEnumerable)
                doubleList.Add(Convert.ToDouble(obj));
            return doubleList.ToArray();
        }

        public static void ValidateExpression<T>(
          EditContext editContext,
          Expression<Func<T>> expression)
        {
            if (expression == null || editContext == null)
                return;
            EditContext editContext1 = editContext;
            FieldIdentifier fieldIdentifier = FieldIdentifier.Create<T>(expression);
            ref FieldIdentifier local = ref fieldIdentifier;
            editContext1.NotifyFieldChanged(in local);
        }

        public static async Task ImportModule(
          IJSRuntime jsRuntime,
          CellScriptModules scriptModule,
          string hashKey = "")
        {
            if (CellBase.DisableScriptManager)
                return;
            string str1 = hashKey != string.Empty ? "-" : hashKey;
            string str2 = CellBaseUtils.GetEnumValue<CellScriptModules>(scriptModule) + str1 + hashKey;
            await jsRuntime.InvokeVoidAsync("import", (object)("./_content/Cell.Blazor/scripts/" + str2 + ".min.js"));
        }

        public static async Task ImportModules(
          IJSRuntime jsRuntime,
          List<ScriptModules> scriptModules,
          string hashKey = "")
        {
            if (CellBase.DisableScriptManager || scriptModules.Count <= 0)
                return;
            List<Task> taskList = new List<Task>();
            foreach (ScriptModules scriptModule in scriptModules)
            {
                string str = CellBaseUtils.GetEnumValue<ScriptModules>(scriptModule) + "-" + hashKey;
                taskList.Add(CellBaseUtils.ImportScripts(jsRuntime, "./_content/Cell.Blazor/scripts/" + str + ".min.js"));
            }
            await Task.WhenAll((IEnumerable<Task>)taskList);
        }

        public static async Task ImportScripts(IJSRuntime jsRuntime, string modulePath) => await jsRuntime.InvokeVoidAsync("import", (object)modulePath);

        public static async Task ImportScripts(
          IJSRuntime jsRuntime,
          ScriptModules scriptModule,
          string hashKey)
        {
            string str = CellBaseUtils.GetEnumValue<ScriptModules>(scriptModule) + "-" + hashKey;
            await jsRuntime.InvokeVoidAsync("import", (object)("./_content/Cell.Blazor/scripts/" + str + ".min.js"));
        }

        public static async Task InvokeDeviceMode(
            CellBlazorService CellService,
          IJSRuntime jsRuntime = null,
          bool icellirst = false)
        {
            if (CellService.IsScriptRendered)
                await Task.CompletedTask;
            else if (!CellService.IsDeviceInvoked)
            {
                CellService.IsDeviceInvoked = true;
                CellBlazorService CellBlazorService = CellService;
                IJSRuntime jsRuntime1 = jsRuntime;
                object[] objArray = new object[1]
                {
          (object) CellService.IsRtlEnabled
                };
                CellBlazorService.IsDeviceMode = await CellBaseUtils.InvokeMethod<bool>(jsRuntime1, "cellBlazor.isDevice", objArray);
                CellBlazorService = (CellBlazorService)null;
                CellService.IsScriptRendered = true;
            }
            else
            {
                await Task.Delay(10);
                await CellBaseUtils.InvokeDeviceMode(CellService);
            }
        }

        public static async Task InvokeEvent<T>(object eventFn, T eventArgs)
        {
            if (eventFn == null)
                return;
            EventCallback<T> eventCallback = (EventCallback<T>)eventFn;
            if (!eventCallback.HasDelegate)
                return;
            await eventCallback.InvokeAsync(eventArgs);
        }

        public static object ChangeType(object dataValue, Type conversionType, bool isClientChange = false)
        {
            Type type = dataValue?.GetType();
            bool flag = type != (Type)null && type.Namespace != null && (type.Namespace.Contains("Collections") || type.IsArray || type.BaseType != (Type)null && type.BaseType.Namespace != null && (type.BaseType.Namespace.Contains("Collections") || type.BaseType.IsArray));
            if (dataValue == null)
                return (object)null;
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                conversionType = Nullable.GetUnderlyingType(conversionType);
            if (conversionType.IsEnum)
            {
                if (!Enum.IsDefined(conversionType, (object)dataValue.ToString()))
                {
                    IEnumerable<string> source1 = ((IEnumerable<string>)Enum.GetNames(conversionType)).Where<string>((Func<string, bool>)(enumValue => enumValue.ToLower() == dataValue.ToString()));
                    IEnumerable<FieldInfo> source2 = ((IEnumerable<FieldInfo>)conversionType.GetFields()).Where<FieldInfo>((Func<FieldInfo, bool>)(x => x.GetCustomAttribute<DisplayAttribute>()?.Name == dataValue.ToString()));
                    dataValue = source2.Count<FieldInfo>() > 0 ? (object)source2.FirstOrDefault<FieldInfo>().Name : (object)source1.FirstOrDefault<string>();
                }
                return dataValue != null ? Enum.Parse(conversionType, dataValue.ToString()) : (object)null;
            }
            if (dataValue.GetType().Name == conversionType.Name)
                return Convert.ChangeType(dataValue, conversionType);
            if (conversionType.IsPrimitive && !conversionType.IsArray && !conversionType.Namespace.Contains("Collections") || (conversionType == typeof(Decimal) || conversionType == typeof(string) || conversionType == typeof(object) && !flag) || conversionType == typeof(DateTime))
                dataValue = (object)Convert.ToString(dataValue, (IFormatProvider)CultureInfo.InvariantCulture);
            else if (conversionType == typeof(Guid))
            {
                dataValue = (object)new Guid(dataValue.ToString());
            }
            else
            {
                if (flag || conversionType.IsInterface && !isClientChange)
                    return dataValue;
                if (conversionType.Name != "DateTimeOffset")
                {
                    string str = conversionType.Name != "TimeSpan" ? dataValue.ToString() : JsonConvert.SerializeObject(dataValue);
                    dataValue = JsonConvert.DeserializeObject(type == typeof(JsonElement) ? BaseComponent.ConvertJsonString(dataValue) : str, conversionType, new JsonSerializerSettings()
                    {
                        DateTimeZoneHandling = DateTimeZoneHandling.Local
                    });
                }
            }
            return Convert.ChangeType(dataValue, conversionType, (IFormatProvider)CultureInfo.InvariantCulture);
        }

        public static string GenerateID(string name = null) => (!string.IsNullOrEmpty(name) ? name + "-" : string.Empty) + Guid.NewGuid().ToString();

        public static async Task InvokeMethod(
          IJSRuntime jsRuntime,
          string methodName,
          params object[] methodParams)
        {
            try
            {
                if (jsRuntime is IJSInProcessRuntime jsRuntime1)
                    jsRuntime1.InvokeVoid(methodName, methodParams);
                else
                    await jsRuntime.InvokeVoidAsync(methodName, methodParams);
            }
            catch (Exception ex)
            {
            }
        }

        public static async ValueTask<T> InvokeMethod<T>(
          IJSRuntime jsRuntime,
          string methodName,
          params object[] methodParams)
        {
            try
            {
                return jsRuntime is IJSInProcessRuntime inProcessRuntime ? inProcessRuntime.Invoke<T>(methodName, methodParams) : await jsRuntime.InvokeAsync<T>(methodName, methodParams);
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        public static int CompareValues<T>(T value1, T value2) => Comparer<T>.Default.Compare(value1, value2);

        public static string AddClass(string prevClass, string className)
        {
            string str = string.IsNullOrEmpty(prevClass) ? string.Empty : prevClass.Trim();
            return str.Contains(className) ? str : str + " " + className;
        }

        public static string RemoveClass(string prevClass, string className)
        {
            string str = string.IsNullOrEmpty(prevClass) ? string.Empty : prevClass.Trim();
            if (!string.IsNullOrEmpty(className))
                str = str.Contains(className) ? prevClass.Replace(className, string.Empty) : str;
            return str;
        }

        public static T[] AddArrayValue<T>(T[] arrayValue, T addValue) => ((IEnumerable<T>)arrayValue).Concat<T>((IEnumerable<T>)new T[1]
        {
      addValue
        }).ToArray<T>();

        public static T[] RemoveArrayValue<T>(T[] arrayValue, T removeValue) => ((IEnumerable<T>)arrayValue).Where<T>((Func<T, bool>)(val => !CellBaseUtils.Equals<T>(val, removeValue))).ToArray<T>();

        public static bool IsNotNullOrEmpty(IList list) => list != null && list.Count > 0;

        public static string FormatUnit(string propertyValue) => propertyValue == "auto" || propertyValue.EndsWith("%", StringComparison.Ordinal) || (propertyValue.EndsWith("px", StringComparison.Ordinal) || propertyValue.EndsWith("vh", StringComparison.Ordinal)) || (propertyValue.EndsWith("vm", StringComparison.Ordinal) || propertyValue.EndsWith("vmax", StringComparison.Ordinal) || (propertyValue.EndsWith("vmin", StringComparison.Ordinal) || propertyValue.EndsWith("em", StringComparison.Ordinal))) ? propertyValue : propertyValue + "px";

        public static string GetEnumValue<T>(T enumValue) where T : struct, IConvertible
        {
            MemberInfo element = typeof(T).GetTypeInfo().DeclaredMembers.SingleOrDefault<MemberInfo>((Func<MemberInfo, bool>)(x => x.Name == enumValue.ToString()));
            if ((object)element == null)
                return (string)null;
            return element.GetCustomAttribute<EnumMemberAttribute>(false)?.Value;
        }

        public static async Task EnableRtl(IJSRuntime jSRuntime, bool isEnabled) => await CellBaseUtils.InvokeMethod(jSRuntime, "cell.base.enableRtl", (object)isEnabled);
    }
}