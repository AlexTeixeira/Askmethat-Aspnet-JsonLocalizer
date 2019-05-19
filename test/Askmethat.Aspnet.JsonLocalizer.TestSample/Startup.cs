using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Askmethat.Aspnet.JsonLocalizer.Extensions;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace Askmethat.Aspnet.JsonLocalizer.TestSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            _ = services.AddMvc()
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_2)
                .AddMvcLocalization();

            CultureInfo[] supportedCultures = new[]
                {
                        new CultureInfo("en-US"),
                        new CultureInfo("fr-FR"),
                        new CultureInfo("pt-PT")
                };

            _ = services.AddJsonLocalization(options =>
            {
                options.ResourcesPath = "json";
                options.UseBaseName = true;
                options.CacheDuration = TimeSpan.FromSeconds(15);
            });

            _ = services.Configure<RequestLocalizationOptions>(options =>
            {

                options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                _ = app.UseDeveloperExceptionPage();
            }

            //app.
            _ = app.UseRequestLocalization();
            _ = app.UseMvcWithDefaultRoute();
        }
    }
}
