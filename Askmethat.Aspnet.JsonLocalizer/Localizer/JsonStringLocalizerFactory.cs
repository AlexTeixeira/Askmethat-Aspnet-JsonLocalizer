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
    internal class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        readonly IHostingEnvironment _env;
        readonly IMemoryCache _memCache;
        readonly IOptions<JsonLocalizationOptions> _localizationOptions;

        readonly string _resourcesRelativePath;
        public JsonStringLocalizerFactory(IHostingEnvironment env, IMemoryCache memCache)
        {
            _env = env;
            _memCache = memCache;
        }

        public JsonStringLocalizerFactory(
                IHostingEnvironment env,
                IMemoryCache memCache,
                IOptions<JsonLocalizationOptions> localizationOptions)
        {
            if (localizationOptions == null)
            {
                throw new ArgumentNullException(nameof(localizationOptions));
            }
            _env = env;
            _memCache = memCache;
            _localizationOptions = localizationOptions;
            _resourcesRelativePath = _localizationOptions.Value.ResourcesPath ?? String.Empty;
        }


        public IStringLocalizer Create(Type resourceSource)
        {
            return  (IStringLocalizer)new JsonStringLocalizer(_env, _memCache, _resourcesRelativePath, _localizationOptions);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return (IStringLocalizer)new JsonStringLocalizer(_env, _memCache, _resourcesRelativePath, _localizationOptions);
        }
    }
}
