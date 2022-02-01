using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Text;
using Askmethat.Aspnet.JsonLocalizer.BlazorWebAssembly.Commons;
using Askmethat.Aspnet.JsonLocalizer.Extensions;
using Askmethat.Aspnet.JsonLocalizer.JsonOptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace Askmethat.Aspnet.JsonLocalizer.Sample.BlazorWebAssembly
{
    public class Program
    {
        private static WebAssemblyHostBuilder _builder;
        private static WebAssemblyHost _host;
        
        private static HashSet<CultureInfo> _supportedCultures;
        private static RequestCulture _defaultRequestCulture;
        
        public static async Task Main(string[] args)
        {
            _builder = WebAssemblyHostBuilder.CreateDefault(args);
            _builder.RootComponents.Add<App>("#app");
            _builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(_builder.HostEnvironment.BaseAddress) });

            ConfigureServices(_builder.Services);
            await ConfigureApplication(_builder.Services);
            
            await _host.RunAsync();
        }

        private static async Task ConfigureApplication(IServiceCollection services)
        {
            
            _host = _builder.Build();

            // var jsInterop = _host.Services.GetRequiredService<IJSRuntime>();
            // var result = await jsInterop.InvokeAsync<string>("blazorCulture.get");
            // if (result != null)
            // {
            //     var culture = new CultureInfo(result);
            //     CultureInfo.DefaultThreadCurrentCulture = culture;
            //     CultureInfo.DefaultThreadCurrentUICulture = culture;
            // }
        }
        
        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped(
                sp => new HttpClient {BaseAddress = new Uri(_builder.HostEnvironment.BaseAddress)});
            // Example of loading a configuration as configuration isn't available yet at this stage.
            services.AddSingleton(provider =>
            {
                var config = provider.GetService<IConfiguration>();
                return config.GetSection("App").Get<AppConfiguration>();
            });
            
            _defaultRequestCulture = new RequestCulture("en-US","en-US");
            _supportedCultures = new HashSet<CultureInfo>
            {
                new CultureInfo("en-US"), new CultureInfo("fr-FR"), new CultureInfo("pt-PT")
            };

            _ = services.AddJsonLocalization(options =>
            {
                options.LocalizationMode = LocalizationMode.BlazorWasm;
                options.UseBaseName = false;
                options.CacheDuration = TimeSpan.FromMinutes(1);
                options.SupportedCultureInfos = _supportedCultures;
                options.FileEncoding = new UTF8Encoding();
                options.IsAbsolutePath = true;
                options.JsonFileList = new[] { "I18n/localization.en.json" };
            });

            _ = services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = _defaultRequestCulture;
                // Formatting numbers, dates, etc.
                options.SupportedCultures = _supportedCultures.ToList();
                // UI strings that we have localized.
                options.SupportedUICultures = _supportedCultures.ToList();
            });
        }
    }
}