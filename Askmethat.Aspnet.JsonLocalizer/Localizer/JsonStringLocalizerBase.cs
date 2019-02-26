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
        protected Dictionary<string, LocalizatedFormat> localization;
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
            var currentCulture = CultureInfo.CurrentUICulture;
            //Look for cache key.
            if (!_memCache.TryGetValue($"{CACHE_KEY}_{currentCulture.ThreeLetterISOLanguageName}", out localization))
            {
                ConstructLocalizationObject(_resourcesRelativePath, currentCulture);
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
        void ConstructLocalizationObject(string jsonPath, CultureInfo currentCulture)
        {
            //be sure that localization is always initialized
            if (localization == null)
            {
                localization = new Dictionary<string, LocalizatedFormat>();
            }

            string pattern = string.IsNullOrWhiteSpace(_baseName) ? "*.json" : $"{_baseName}/*.json";
            //get all files ending by json extension
            var myFiles = Directory.GetFiles(jsonPath, pattern, SearchOption.AllDirectories);

            foreach (string file in myFiles)
            {
                var tempLocalization = JsonConvert.DeserializeObject<Dictionary<string, JsonLocalizationFormat>>(File.ReadAllText(file, _localizationOptions.Value.FileEncoding));
                foreach (var temp in tempLocalization)
                {
                    var localizedValue = GetLocalizedValue(currentCulture, temp);
                    if (!string.IsNullOrEmpty(localizedValue.Value))
                    {
                        if (!localization.ContainsKey(temp.Key))
                        {
                            localization.Add(temp.Key, localizedValue);
                        }
                        else if (localization[temp.Key].IsParent)
                        {
                            localization[temp.Key] = localizedValue;
                        }
                    }
                }
            }
        }

        private LocalizatedFormat GetLocalizedValue(CultureInfo currentCulture, KeyValuePair<string, JsonLocalizationFormat> temp)
        {
            bool isParent = false;
            var value = temp.Value.Values.FirstOrDefault(s => string.Equals(s.Key, currentCulture.Name, StringComparison.InvariantCultureIgnoreCase)).Value;
            if (string.IsNullOrEmpty(value))
            {
                isParent = true;
                value = temp.Value.Values.FirstOrDefault(s => string.Equals(s.Key, currentCulture.Parent.Name, StringComparison.InvariantCultureIgnoreCase)).Value;
                if (string.IsNullOrEmpty(value))
                {
                    value = temp.Value.Values.FirstOrDefault(s => string.IsNullOrWhiteSpace(s.Key)).Value;
                    if (string.IsNullOrEmpty(value) && _localizationOptions.Value.DefaultCulture != null)
                    {
                        value = temp.Value.Values.FirstOrDefault(s => string.Equals(s.Key, _localizationOptions.Value.DefaultCulture.Name, StringComparison.InvariantCultureIgnoreCase)).Value;
                        if (string.IsNullOrEmpty(value))
                        {
                            value = null;
                        }
                    }
                }
            }
            return new LocalizatedFormat()
            {
                IsParent = isParent,
                Value = value
            };
        }

        string TransformBaseNameToPath(string baseName)
        {
            string friendlyName = string.Empty;

            friendlyName = AppDomain.CurrentDomain.FriendlyName;

            return baseName.Replace($"{friendlyName}.", "").Replace(".", "/");
        }

    }
}