using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Askmethat.Aspnet.JsonLocalizer.Extensions;
using Askmethat.Aspnet.JsonLocalizer.Format;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Askmethat.Aspnet.JsonLocalizer.Localizer
{
    internal class JsonStringLocalizerBase
    {
        protected Dictionary<string, LocalizationFormat> localization;
        protected readonly IMemoryCache _memCache;
        protected readonly IOptions<JsonLocalizationOptions> _localizationOptions;
        protected readonly string _resourcesRelativePath;
        protected readonly string _baseName;

        protected readonly TimeSpan _memCacheDuration;
        protected const string CACHE_KEY = "LocalizationBlob";

        public JsonStringLocalizerBase(string resourcesRelativePath, IOptions<JsonLocalizationOptions> localizationOptions, string baseName = null)
        {
            _resourcesRelativePath = resourcesRelativePath;
            _baseName = TransformBaseNameToPath(baseName);
            _localizationOptions = localizationOptions;
            _memCache = _localizationOptions.Value.Caching;
            _memCacheDuration = _localizationOptions.Value.CacheDuration;
            InitJsonStringLocalizer();
        }

        public JsonStringLocalizerBase(IOptions<JsonLocalizationOptions> localizationOptions)
        {
            _localizationOptions = localizationOptions;
            _resourcesRelativePath = _localizationOptions.Value.ResourcesPath ?? String.Empty;
            _memCacheDuration = _localizationOptions.Value.CacheDuration;
            _memCache = _localizationOptions.Value.Caching;
            InitJsonStringLocalizer();
        }

        void InitJsonStringLocalizer()
        {
            //Look for cache key.
            if (!_memCache.TryGetValue(CACHE_KEY, out localization))
            {
                ConstructLocalizationObject(_resourcesRelativePath);
                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(_memCacheDuration);

                // Save data in cache.
                _memCache.Set(CACHE_KEY, localization, cacheEntryOptions);
            }
        }

        /// <summary>
        /// Construct localization object from json files
        /// </summary>
        /// <param name="jsonPath">Json file path</param>
        void ConstructLocalizationObject(string jsonPath)
        {
            //be sure that localization is always initialized
            if (localization == null)
            {
                localization = new Dictionary<string, LocalizationFormat>();
            }

            string pattern = string.IsNullOrWhiteSpace(_baseName) ? "*.json" : $"{_baseName}/*.json";
            //get all files ending by json extension
            var myFiles = Directory.GetFiles(jsonPath, pattern, SearchOption.AllDirectories);

            foreach (string file in myFiles)
            {
                var tempLocalization = JsonConvert.DeserializeObject<Dictionary<string, JsonLocalizationFormat>>(File.ReadAllText(file, _localizationOptions.Value.FileEncoding));

                foreach (var temp in tempLocalization)
                {

                    if (localization.ContainsKey(temp.Key))
                    {
                        localization[temp.Key].Values = localization[temp.Key].Values.Concat(temp.Value.Values.ToDictionary(s => new CultureInfo(s.Key).LCID, s => s.Value))
                                                        .ToDictionary(s => s.Key, s => s.Value);
                    }
                    else
                    {
                        var currentLocalization = new LocalizationFormat();
                        currentLocalization.Values = temp.Value.Values.ToDictionary(s => new CultureInfo(s.Key).LCID, s => s.Value);
                        localization.Add(temp.Key, currentLocalization);
                    }
                }
            }
        }

        string TransformBaseNameToPath(string baseName)
        {
            string friendlyName = string.Empty;

            friendlyName = AppDomain.CurrentDomain.FriendlyName;

            return baseName.Replace($"{friendlyName}.", "").Replace(".", "/");
        }

    }
}