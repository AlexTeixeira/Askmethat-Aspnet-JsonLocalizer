using Askmethat.Aspnet.JsonLocalizer.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace Askmethat.Aspnet.JsonLocalizer.Localizer
{
    //resource not use, only here to match microsoft interfaces
    internal class JsonStringLocalizerOfT<T> : JsonStringLocalizer, IJsonStringLocalizer<T>
    {
        public JsonStringLocalizerOfT(IOptions<JsonLocalizationOptions> localizationOptions, IHostingEnvironment env, string baseName = null) : base(localizationOptions, env, baseName)
        {
        }
    }
}