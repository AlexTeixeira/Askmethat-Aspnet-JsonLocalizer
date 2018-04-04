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
    internal class JsonStringLocalizer : IStringLocalizer
    {
        List<JsonLocalizationFormat> localization = new List<JsonLocalizationFormat>();
        private readonly IHostingEnvironment _env;
        private readonly IMemoryCache _memCache;
        private readonly string _resourcesRelativePath;
        private readonly TimeSpan _memCacheDuration;
        private const string CACHE_KEY = "LocalizationBlob";

        public JsonStringLocalizer(IHostingEnvironment env, IMemoryCache memCache, string resourcesRelativePath)
        {
            _env = env;
            _memCache = memCache;
            _resourcesRelativePath = resourcesRelativePath;
            _memCacheDuration = TimeSpan.FromMinutes(30);
            InitJsonStringLocalizer();
        }


        public JsonStringLocalizer(IHostingEnvironment env, IMemoryCache memCache, IOptions<JsonLocalizationOptions> localizationOptions)
        {
            _env = env;
            _memCache = memCache;
            _resourcesRelativePath = localizationOptions.Value.ResourcesPath ?? String.Empty;
            _memCacheDuration = localizationOptions.Value.CacheDuration;

            InitJsonStringLocalizer();
        }

        private void InitJsonStringLocalizer()
        {

            string jsonPath = GetJsonRelativePath();
            //read all json file
            JsonSerializer serializer = new JsonSerializer();

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
        private void ConstructLocalizationObject(string jsonPath)
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
                localization.AddRange(JsonConvert.DeserializeObject<List<JsonLocalizationFormat>>(File.ReadAllText(file)));
            }


            MergeValues();

        }

        private void MergeValues()
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
                var value = GetString(name);
                return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var format = GetString(name);
                var value = string.Format(format ?? name, arguments);
                return new LocalizedString(name, value, resourceNotFound: format == null);
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return localization.Where(l => l.Values.Keys.Any(lv => lv == CultureInfo.CurrentCulture.Name)).Select(l => new LocalizedString(l.Key, l.Values[CultureInfo.CurrentCulture.Name], true));
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return new JsonStringLocalizer(_env, _memCache, _resourcesRelativePath);
        }

        /// <summary>
        /// Get the string from JSON cached file
        /// </summary>
        /// <param name="name">Value name</param>
        /// <returns>Value if thing</returns>
        private string GetString(string name)
        {

            var query = localization.Where(l => l.Values.Keys.Any(lv => lv == CultureInfo.CurrentCulture.Name));
            var value = query.FirstOrDefault(l => l.Key == name);

            if (value == null)
                return string.Empty;

            return value.Values[CultureInfo.CurrentCulture.Name];
        }

    }
}
