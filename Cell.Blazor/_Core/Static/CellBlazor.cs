using System;
using System.Linq;
using System.Net.Http;
using Cell.Blazor._Core.Interface;
using Cell.Blazor._Core.Service;
using Cell.Blazor.Internal.Class;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Cell.Blazor._Core.Static
{
    public static class CellBlazor
    {
        public static IServiceCollection AddSyncfusionBlazor(
            this IServiceCollection services,
            bool DisableScriptManager = false)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<ICellBlazor, CellBlazorService>();
            services.TryAddSingleton<CellStringLocalizer, CellStringLocalizer>();
            services.AddScoped<CellBlazorService>();
            CellBase.DisableScriptManager = DisableScriptManager;
            if (!services.Any<ServiceDescriptor>((Func<ServiceDescriptor, bool>)(s => s.ServiceType == typeof(HttpClient))))
                services.AddScoped<HttpClient>();
            return services;
        }
    }
}