using Askmethat.Aspnet.JsonLocalizer.Extensions;
using Askmethat.Aspnet.JsonLocalizer.Format;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;


namespace Askmethat.Aspnet.JsonLocalizer.Localizer
{
    /// <summary>
    /// Json String localizer
    /// Used to read JSON File and add it to cache ( default 30 minutes )
    /// </summary>
    internal class JsonFallbackStringLocalizer : IStringLocalizer
    {
        List<JsonLocalizationFormat> localization = new List<JsonLocalizationFormat>();
        readonly IHostingEnvironment _env;
        readonly IMemoryCache _memCache;
        readonly IOptions<JsonLocalizationOptions> _localizationOptions;
        readonly string _resourcesRelativePath;
        readonly TimeSpan _memCacheDuration;
        const string CACHE_KEY = "LocalizationBlob";

        public JsonFallbackStringLocalizer(IHostingEnvironment env, IMemoryCache memCache, string resourcesRelativePath, IOptions<JsonLocalizationOptions> localizationOptions)
        {
            _env = env;
            _memCache = memCache;
            _resourcesRelativePath = resourcesRelativePath;
            _localizationOptions = localizationOptions;
            _memCacheDuration = _localizationOptions.Value.CacheDuration;
            InitJsonStringLocalizer();
        }


        public JsonFallbackStringLocalizer(IHostingEnvironment env, IMemoryCache memCache, IOptions<JsonLocalizationOptions> localizationOptions)
        {
            _env = env;
            _memCache = memCache;
            _localizationOptions = localizationOptions;
            _resourcesRelativePath = _localizationOptions.Value.ResourcesPath ?? String.Empty;
            _memCacheDuration = _localizationOptions.Value.CacheDuration;

            InitJsonStringLocalizer();
        }

        void InitJsonStringLocalizer()
        {
            string jsonPath = GetJsonRelativePath();

            // Look for cache key.
            if (!_memCache.TryGetValue(CACHE_KEY, out localization))
            {
                ConstructLocalizationObject(jsonPath);
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
                localization = new List<JsonLocalizationFormat>();
            }

            //get all files ending by json extension
            var myFiles = Directory.GetFiles(jsonPath, "*.json", SearchOption.AllDirectories);

            foreach (string file in myFiles)
            {
                localization.AddRange(JsonConvert.DeserializeObject<List<JsonLocalizationFormat>>(File.ReadAllText(file, _localizationOptions.Value.FileEncoding)));
            }


            MergeValues();

        }

        /// <summary>
        /// Merge value to avoid duplicate culture in list
        /// </summary>
        void MergeValues()
        {
            var groups = localization.GroupBy(g => g.Key);

            var tempLocalization = new List<JsonLocalizationFormat>();

            foreach (var group in groups)
            {
                try
                {
                    var jsonValues = group
                        .Select(s => s.Values)
                        .SelectMany(dict => dict)
                        .ToDictionary(t => t.Key, t => t.Value);

                    tempLocalization.Add(new JsonLocalizationFormat()
                    {
                        Key = group.Key,
                        Values = jsonValues
                    });
                }
                catch (Exception e)
                {
                    throw new ArgumentException($"{group.Key} could not contains two translation for the same language code", e);
                }

            }

            //merged values
            localization = tempLocalization;
        }

        /// <summary>
        /// Get path of json
        /// </summary>
        /// <returns>JSON relative path</returns>
        string GetJsonRelativePath()
        {
            return !string.IsNullOrEmpty(_resourcesRelativePath) ? $"{GetBuildPath()}/{_resourcesRelativePath}/" : $"{_env.ContentRootPath}/Resources/";
        }

        string GetBuildPath()
        {
            return AppContext.BaseDirectory;
        }

        public LocalizedString this[string name]
        {
            get
            {
                var value = GetStringSafely(name);
                return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var format = GetStringSafely(name);
                var value = String.Format(format ?? name, arguments);
                return new LocalizedString(name, value, resourceNotFound: format == null);
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return includeParentCultures
                ? localization
                    .Select(
                        l => {
                            var value = GetStringSafely(l.Key);
                            return new LocalizedString(l.Key, value ?? l.Key, resourceNotFound: value == null);
                        }
                    )
                : localization
                    .Where(l => l.Values.Keys.Any(lv => lv == CultureInfo.CurrentCulture.Name))
                    .Select(l => new LocalizedString(l.Key, l.Values[CultureInfo.CurrentCulture.Name], false));
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return new JsonStringLocalizer(_env, _memCache, _resourcesRelativePath, _localizationOptions);
        }

        string GetStringSafely(string name, CultureInfo cultureInfo = null)
        {
            if (name == null) {
                throw new ArgumentNullException(nameof(name));
            }

            if (cultureInfo == null) {
                cultureInfo = CultureInfo.CurrentCulture;
            }

            var valuesSection = localization
                .Where(l => l.Values.ContainsKey(cultureInfo.Name))
                .FirstOrDefault(l => l.Key == name);

            if (valuesSection != null) {
                return valuesSection.Values[cultureInfo.Name];
            }

            if (!cultureInfo.Equals(cultureInfo.Parent)) {
                //Try the parent culture
                return GetStringSafely(name, cultureInfo.Parent);
            }

            return null;
        }
    }
}
