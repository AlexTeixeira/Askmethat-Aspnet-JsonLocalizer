using Askmethat.Aspnet.JsonLocalizer.JsonOptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Askmethat.Aspnet.JsonLocalizer.Localizer
{
    //resource not use, only here to match microsoft interfaces
    internal class JsonStringLocalizerOfT<T> : JsonStringLocalizer, IJsonStringLocalizer<T>, IStringLocalizer<T>
    {
#if NETCORE
         public JsonStringLocalizerOfT(IOptions<JsonLocalizationOptions> localizationOptions, IWebHostEnvironment env, string baseName
 = null) : base(localizationOptions, env, ModifyBaseName)
        {
        }
#elif WEBASSEMBLY
            public JsonStringLocalizerOfT(IOptions<JsonLocalizationOptions> localizationOptions, IWebAssemblyHostEnvironment env, string baseName
 = null) : base(localizationOptions, env, ModifyBaseName)
        {
        }
#else
        public JsonStringLocalizerOfT(IOptions<JsonLocalizationOptions> localizationOptions, IHostingEnvironment env,
            string baseName = null) : base(localizationOptions, env, ModifyBaseName)
        {
        }
#endif

        private static string ModifyBaseName => typeof(T).ToString();
    }
}