﻿using Askmethat.Aspnet.JsonLocalizer.Extensions;
using Askmethat.Aspnet.JsonLocalizer.Format;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Askmethat.Aspnet.JsonLocalizer.JsonOptions;
using System.Net.Http;
#if NETCORE
using Microsoft.AspNetCore.Components;
#endif

namespace Askmethat.Aspnet.JsonLocalizer.Localizer
{
    internal partial class JsonStringLocalizer : JsonStringLocalizerBase, IJsonStringLocalizer
    {

#if NETCORE
        private readonly EnvironmentWrapper _env;

        public JsonStringLocalizer(IOptions<JsonLocalizationOptions> localizationOptions, EnvironmentWrapper env, string baseName
= null) : base(localizationOptions, env, baseName)
        {
            _env = env;
            _missingTranslations = localizationOptions.Value.MissingTranslationsOutputFile;
            resourcesRelativePaths.Add(GetJsonRelativePath(_localizationOptions.Value.ResourcesPath));
            if (_localizationOptions.Value.AdditionalResourcePaths != null)
            {
                foreach (var path in _localizationOptions.Value.AdditionalResourcePaths)
                {
                    resourcesRelativePaths.Add(GetJsonRelativePath(path));
                }
            }
        }
#else

        private readonly IHostingEnvironment _env;

        public JsonStringLocalizer(IOptions<JsonLocalizationOptions> localizationOptions, IHostingEnvironment env, string baseName = null) : base(localizationOptions, null, baseName)
        {
            _env = env;
            _missingTranslations = localizationOptions.Value.MissingTranslationsOutputFile;
            resourcesRelativePaths.Add(GetJsonRelativePath(_localizationOptions.Value.ResourcesPath));
            if (_localizationOptions.Value.AdditionalResourcePaths != null)
            {
                foreach (var path in _localizationOptions.Value.AdditionalResourcePaths)
                {
                    resourcesRelativePaths.Add(GetJsonRelativePath(path));
                }
            }            
        }
#endif

    }
}