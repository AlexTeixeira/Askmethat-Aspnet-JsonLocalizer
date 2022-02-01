using Askmethat.Aspnet.JsonLocalizer.Extensions;
#if NETSTANDARD
using Microsoft.AspNetCore.Hosting;
#endif
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using Askmethat.Aspnet.JsonLocalizer.JsonOptions;

namespace Askmethat.Aspnet.JsonLocalizer.Localizer
{
    /// <summary>
    /// Factory the create the JsonStringLocalizer
    /// </summary>
    public class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly IOptions<JsonLocalizationOptions> _localizationOptions;

#if NETCORE
            private readonly EnvironmentWrapper _env;
        
         public JsonStringLocalizerFactory(
            EnvironmentWrapper env,
            IOptions<JsonLocalizationOptions> localizationOptions = null)
        {
            _env = env;
            _localizationOptions = localizationOptions ?? throw new ArgumentNullException(nameof(localizationOptions));
        }
#else

        //private readonly IHostingEnvironment _env;
        private readonly IHostingEnvironment _env;

        public JsonStringLocalizerFactory(
            IHostingEnvironment env,
            IOptions<JsonLocalizationOptions> localizationOptions = null)
        {
            _env = env;
            _localizationOptions = localizationOptions ?? throw new ArgumentNullException(nameof(localizationOptions));
        }
#endif


        public IStringLocalizer Create(Type resourceSource)
        {
            return new JsonStringLocalizer(_localizationOptions, _env, null);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            baseName = _localizationOptions.Value.UseBaseName ? baseName : string.Empty;
            return new JsonStringLocalizer(_localizationOptions, _env, null, baseName);
        }
    }
}