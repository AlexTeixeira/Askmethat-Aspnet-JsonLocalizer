using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Askmethat.Aspnet.JsonLocalizer.Localizer
{
    /// <summary>
    /// Factory the create the JsonStringLocalizer
    /// </summary>
    internal class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly IHostingEnvironment _env;
        private readonly IMemoryCache _memCache;
        private readonly string _resourcesRelativePath;
        public JsonStringLocalizerFactory(IHostingEnvironment env, IMemoryCache memCache)
        {
            _env = env;
            _memCache = memCache;
        }

        public JsonStringLocalizerFactory(
                IHostingEnvironment env,
                IMemoryCache memCache,
                IOptions<LocalizationOptions> localizationOptions)
        {
            if (localizationOptions == null)
            {
                throw new ArgumentNullException(nameof(localizationOptions));
            }
            _env = env;
            _memCache = memCache;

            _resourcesRelativePath = localizationOptions.Value.ResourcesPath ?? String.Empty;
        }


        public IStringLocalizer Create(Type resourceSource)
        {
            return new JsonStringLocalizer(_env, _memCache, _resourcesRelativePath);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new JsonStringLocalizer(_env, _memCache, _resourcesRelativePath);
        }
    }
}
