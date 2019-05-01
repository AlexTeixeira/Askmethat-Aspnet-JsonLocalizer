using Askmethat.Aspnet.JsonLocalizer.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;

namespace Askmethat.Aspnet.JsonLocalizer.Localizer
{
    /// <summary>
    /// Factory the create the JsonStringLocalizer
    /// </summary>
    public class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        readonly IHostingEnvironment _env;
        readonly IOptions<JsonLocalizationOptions> _localizationOptions;

        public JsonStringLocalizerFactory(
                IHostingEnvironment env,
                IOptions<JsonLocalizationOptions> localizationOptions = null)
        {
            if (localizationOptions == null)
            {
                throw new ArgumentNullException(nameof(localizationOptions));
            }
            _env = env;
            _localizationOptions = localizationOptions;
        }


        public IStringLocalizer Create(Type resourceSource)
        {
            return (IStringLocalizer)new JsonStringLocalizer(_localizationOptions, _env);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            baseName = _localizationOptions.Value.UseBaseName ? baseName : string.Empty;
            return (IStringLocalizer)new JsonStringLocalizer(_localizationOptions, _env, baseName);
        }
    }
}
