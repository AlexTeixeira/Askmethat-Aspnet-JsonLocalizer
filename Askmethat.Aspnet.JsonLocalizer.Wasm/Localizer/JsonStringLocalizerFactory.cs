﻿using Askmethat.Aspnet.JsonLocalizer.Extensions;
#if NETSTANDARD
using Microsoft.AspNetCore.Hosting;
#endif
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using Askmethat.Aspnet.JsonLocalizer.JsonOptions;
using System.Net.Http;

namespace Askmethat.Aspnet.JsonLocalizer.Localizer
{
    /// <summary>
    /// Factory the create the JsonStringLocalizer
    /// </summary>
    public class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly IOptions<JsonLocalizationOptions> _localizationOptions;


            private readonly EnvironmentWrapper _env;
        private readonly HttpClient _httpClient;

        public JsonStringLocalizerFactory(
            EnvironmentWrapper env,
            HttpClient httpClient,
            IOptions<JsonLocalizationOptions> localizationOptions = null)
        {
            _env = env;
            _httpClient = httpClient;
            _localizationOptions = localizationOptions ?? throw new ArgumentNullException(nameof(localizationOptions));
        }



        public IStringLocalizer Create(Type resourceSource)
        {
            return new JsonStringLocalizer(_localizationOptions, _env,_httpClient);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            baseName = _localizationOptions.Value.UseBaseName ? baseName : string.Empty;
            return new JsonStringLocalizer(_localizationOptions, _env, _httpClient, baseName);
        }
    }
}