﻿using Askmethat.Aspnet.JsonLocalizer.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.IO;

namespace Askmethat.Aspnet.JsonLocalizer.Localizer
{
    /// <summary>
    /// Factory the create the JsonStringLocalizer
    /// </summary>
    public class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        readonly IHostingEnvironment _env;
        readonly IOptions<JsonLocalizationOptions> _localizationOptions;

        readonly string _resourcesRelativePath;

        public JsonStringLocalizerFactory(IHostingEnvironment env)
        {
            _env = env;
            _localizationOptions = Options.Create<JsonLocalizationOptions>(new JsonLocalizationOptions { });
            _resourcesRelativePath = _localizationOptions.Value.ResourcesPath ?? String.Empty;
        }

        public JsonStringLocalizerFactory(
                IHostingEnvironment env,
                IOptions<JsonLocalizationOptions> localizationOptions)
        {
            if (localizationOptions == null)
            {
                throw new ArgumentNullException(nameof(localizationOptions));
            }
            _env = env;
            _localizationOptions = localizationOptions;
            _resourcesRelativePath = _localizationOptions.Value.ResourcesPath ?? String.Empty;
        }


        public IStringLocalizer Create(Type resourceSource)
        {
            var path = !string.IsNullOrEmpty(_resourcesRelativePath) ? GetJsonRelativePath(_resourcesRelativePath) : GetJsonRelativePath(_resourcesRelativePath);
            return (IStringLocalizer)new JsonStringLocalizer(path, _localizationOptions);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            baseName = _localizationOptions.Value.UseBaseName ? baseName : string.Empty;
            return (IStringLocalizer)new JsonStringLocalizer(GetJsonRelativePath(_resourcesRelativePath), _localizationOptions, baseName);
        }

        /// <summary>
        /// Get path of json
        /// </summary>
        /// <returns>JSON relative path</returns>
        string GetJsonRelativePath(string path)
        {
            var fullPath = string.Empty;
            if (this._localizationOptions.Value.IsAbsolutePath)
            {
                fullPath = path;
            }
            if (!this._localizationOptions.Value.IsAbsolutePath && string.IsNullOrEmpty(path))
            {
                fullPath = Path.Combine(_env.ContentRootPath, "Resources");
            }
            else if (!this._localizationOptions.Value.IsAbsolutePath && !string.IsNullOrEmpty(path))
            {
                fullPath = Path.Combine(AppContext.BaseDirectory, path);
            }
            return fullPath;
        }
    }
}
