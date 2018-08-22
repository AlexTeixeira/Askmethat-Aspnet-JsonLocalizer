using System;
using System.Collections.Generic;
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
        protected List<JsonLocalizationFormat> localization = new List<JsonLocalizationFormat>();
        protected readonly IHostingEnvironment _env;
        protected readonly IMemoryCache _memCache;
        protected readonly IOptions<JsonLocalizationOptions> _localizationOptions;
        protected readonly string _resourcesRelativePath;
        protected readonly TimeSpan _memCacheDuration;
        protected const string CACHE_KEY = "LocalizationBlob";

        public JsonStringLocalizerBase(IHostingEnvironment env, IMemoryCache memCache, string resourcesRelativePath, IOptions<JsonLocalizationOptions> localizationOptions)
        {
            _env = env;
            _memCache = memCache;
            _resourcesRelativePath = resourcesRelativePath;
            _localizationOptions = localizationOptions;
            _memCacheDuration = _localizationOptions.Value.CacheDuration;
            InitJsonStringLocalizer();
        }

        public JsonStringLocalizerBase(IHostingEnvironment env, IMemoryCache memCache, IOptions<JsonLocalizationOptions> localizationOptions)
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
                        .ToDictionary(t => t.Key, t => t.Value, StringComparer.OrdinalIgnoreCase);

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

    }
}