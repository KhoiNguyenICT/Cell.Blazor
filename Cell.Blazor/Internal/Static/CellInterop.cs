using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cell.Blazor.Internal.Class;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;

namespace Cell.Blazor.Internal.Static
{
    public static class CellInterop
    {
        public static async ValueTask<T> Init<T>(
          IJSRuntime jsRuntime,
          string elementId,
          object model,
          string[] events,
          string nameSpace,
          DotNetObjectReference<object> helper,
          string bindableProps,
          Dictionary<string, object> htmlAttributes = null,
          Dictionary<string, object> templateRefs = null,
          DotNetObjectReference<object> adaptor = null,
          string localeText = null)
        {
            try
            {
                return await jsRuntime.InvokeAsync<T>("cellBlazor.initialize", (object)elementId, model, (object)events, (object)nameSpace, (object)helper, (object)bindableProps, (object)htmlAttributes, (object)templateRefs, (object)adaptor, (object)localeText);
            }
            catch (Exception ex)
            {
                return await LogError<T>(jsRuntime, ex, nameSpace + " - #" + elementId + " - Init had public server error \n");
            }
        }

        public static async ValueTask<T> Update<T>(
          IJSRuntime jsRuntime,
          string elementId,
          string model,
          string nameSpace)
        {
            try
            {
                return await jsRuntime.InvokeAsync<T>("cellBlazor.setModel", (object)elementId, (object)model, (object)nameSpace);
            }
            catch (Exception ex)
            {
                return await LogError<T>(jsRuntime, ex, nameSpace + " - #" + elementId + " - Update model had public server error \n");
            }
        }

        public static async ValueTask<T> InvokeMethod<T>(
          IJSRuntime jsRuntime,
          string elementId,
          string methodName,
          string moduleName,
          object[] args,
          string nameSpace,
          ElementReference? element = null)
        {
            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
                };
                return await jsRuntime.InvokeAsync<T>("cellBlazor.invokeMethod", (object)elementId, (object)methodName, (object)moduleName, (object)JsonConvert.SerializeObject(args, settings), (object)element);
            }
            catch (Exception ex)
            {
                return await LogError<T>(jsRuntime, ex, nameSpace + " - #" + elementId + " - invokeMethod had public server error \n");
            }
        }

        public static ValueTask<T> InvokeGet<T>(
          IJSRuntime jsRuntime,
          string id,
          string moduleName,
          string methodName,
          string nameSpace)
        {
            try
            {
                return CellBaseUtils.InvokeMethod<T>(jsRuntime, "cellBlazor.getMethodCall", (object)id, (object)moduleName, (object)methodName);
            }
            catch (Exception ex)
            {
                string message = nameSpace + " - #" + id + " - getMethodCall had public server error \n";
                return LogError<T>(jsRuntime, ex, message);
            }
        }

        public static ValueTask<T> InvokeSet<T>(
          IJSRuntime jsRuntime,
          string id,
          string moduleName,
          string methodName,
          object[] args,
          string nameSpace)
        {
            try
            {
                return CellBaseUtils.InvokeMethod<T>(jsRuntime, "cellBlazor.setMethodCall", (object)id, (object)moduleName, (object)methodName, (object)args);
            }
            catch (Exception ex)
            {
                string message = nameSpace + " - #" + id + " - setMethodCall had public server error \n";
                return LogError<T>(jsRuntime, ex, message);
            }
        }

        public static async ValueTask<T> SetTemplateInstance<T>(
          IJSRuntime jsRuntime,
          string templateName,
          DotNetObjectReference<object> helper,
          int guid)
        {
            try
            {
                return await CellBaseUtils.InvokeMethod<T>(jsRuntime, "cellBlazor.setTemplateInstance", (object)templateName, (object)helper, (object)guid);
            }
            catch (Exception ex)
            {
                return await LogError<T>(jsRuntime, ex);
            }
        }

        public static async ValueTask<T> SetTemplates<T>(
          IJSRuntime jsRuntime,
          string templateID)
        {
            try
            {
                return await jsRuntime.InvokeAsync<T>("cellBlazor.setTemplate", (object)templateID);
            }
            catch (Exception ex)
            {
                return await LogError<T>(jsRuntime, ex);
            }
        }

        public static ValueTask<T> LogError<T>(
          IJSRuntime jsRuntime,
          Exception e,
          string message = "")
        {
            try
            {
                ErrorMessage errorMessage = new ErrorMessage();
                errorMessage.Message = message + e.Message;
                errorMessage.Stack = e.StackTrace;
                if (e.InnerException != null)
                {
                    errorMessage.Message = message + e.InnerException.Message;
                    errorMessage.Stack = e.InnerException.StackTrace;
                }
                return jsRuntime.InvokeAsync<T>("cellBlazor.throwError", (object)errorMessage);
            }
            catch
            {
                return new ValueTask<T>();
            }
        }

        public static async Task Animate(
          IJSRuntime jsRuntime,
          object reference,
          object animationSettings)
        {
            try
            {
                await CellBaseUtils.InvokeMethod(jsRuntime, "cellBlazor.animate", reference, animationSettings);
            }
            catch (Exception ex)
            {
                object obj = await LogError<object>(jsRuntime, ex);
            }
        }

        public static async Task RippleEffect(
          IJSRuntime jsRuntime,
          object reference,
          object rippleSettings)
        {
            try
            {
                await CellBaseUtils.InvokeMethod(jsRuntime, "cellBlazor.callRipple", reference, rippleSettings);
            }
            catch (Exception ex)
            {
                object obj = await LogError<object>(jsRuntime, ex);
            }
        }
    }
}