using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Linq;
using Askmethat.Aspnet.JsonLocalizer.Extensions;
using Askmethat.Aspnet.JsonLocalizer.JsonOptions;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Hosting;

namespace Askmethat.Aspnet.JsonLocalizer.TestSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public IConfiguration Configuration { get; }

        JsonLocalizationOptions _jsonLocalizationOptions;
        List<CultureInfo> _supportedCultures;
        RequestCulture _defaultRequestCulture;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            _ = services.AddControllersWithViews()
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0)
                .AddDataAnnotationsLocalization()
                .AddViewLocalization();
            _ = services.AddRazorPages()
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0)
                .AddDataAnnotationsLocalization()
                .AddViewLocalization();

            // Get json localization options from appsettings 
            var jsonLocalizationOptions = Configuration.GetSection(nameof(JsonLocalizationOptions));

            _jsonLocalizationOptions = jsonLocalizationOptions.Get<JsonLocalizationOptions>();
            _defaultRequestCulture = new RequestCulture(_jsonLocalizationOptions.DefaultCulture,
                _jsonLocalizationOptions.DefaultUICulture);
            _supportedCultures = _jsonLocalizationOptions.SupportedCultureInfos.ToList();

            _ = services.AddJsonLocalization(options =>
            {
                options.ResourcesPath = _jsonLocalizationOptions.ResourcesPath;
                options.UseBaseName = _jsonLocalizationOptions.UseBaseName;
                options.CacheDuration = _jsonLocalizationOptions.CacheDuration;
                options.SupportedCultureInfos = _jsonLocalizationOptions.SupportedCultureInfos;
                options.FileEncoding = _jsonLocalizationOptions.FileEncoding;
                options.IsAbsolutePath = _jsonLocalizationOptions.IsAbsolutePath;
            });

            _ = services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = _defaultRequestCulture;
                // Formatting numbers, dates, etc.
                options.SupportedCultures = _supportedCultures;
                // UI strings that we have localized.
                options.SupportedUICultures = _supportedCultures;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                _ = app.UseDeveloperExceptionPage();
            }

            _ = app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = _defaultRequestCulture,
                // Formatting numbers, dates, etc.
                SupportedCultures = _supportedCultures,
                // UI strings that we have localized.
                SupportedUICultures = _supportedCultures
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}