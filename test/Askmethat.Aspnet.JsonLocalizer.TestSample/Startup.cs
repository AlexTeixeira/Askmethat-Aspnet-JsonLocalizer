using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Askmethat.Aspnet.JsonLocalizer.Extensions;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace Askmethat.Aspnet.JsonLocalizer.TestSample
{
    public class Startup
    {
#if NETCOREAPP2_0
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public IConfiguration Configuration { get; }
#endif
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMvc().AddViewLocalization();

            services.AddJsonLocalization(options => options.ResourcesPath = "json");

            services.Configure<RequestLocalizationOptions>(options =>  
            {  
                var supportedCultures = new[]  
                {  
                        new CultureInfo("en-US"),  
                        new CultureInfo("fr-FR")  
                };  
        
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
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRequestLocalization();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
