using Askmethat.Aspnet.JsonLocalizer.Extensions;
using Askmethat.Aspnet.JsonLocalizer.Format;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Askmethat.Aspnet.JsonLocalizer.Localizer
{
    internal class JsonStringLocalizerBase
    {
        protected readonly IMemoryCache _memCache;
        protected readonly IOptions<JsonLocalizationOptions> _localizationOptions;
        protected readonly string _baseName;
        protected readonly TimeSpan _memCacheDuration;

        protected const string CACHE_KEY = "LocalizationBlob";
        protected string resourcesRelativePath;
        protected string currentCulture = string.Empty;
        protected Dictionary<string, LocalizatedFormat> localization;

        public JsonStringLocalizerBase(IOptions<JsonLocalizationOptions> localizationOptions, string baseName = null)
        {
            _baseName = TransformBaseNameToPath(baseName);
            _localizationOptions = localizationOptions;
            _memCache = _localizationOptions.Value.Caching;
            _memCacheDuration = _localizationOptions.Value.CacheDuration;
        }

        string GetCacheKey(CultureInfo ci) => $"{CACHE_KEY}_{ci.DisplayName}";

        //string GetCacheKey(CultureInfo ci)
        //{
        //    if (_localizationOptions.Value.UseBaseName)
        //    {
        //        return $"{CACHE_KEY}_{ci.DisplayName}_{_baseName}";
        //    }
        //    return $"{CACHE_KEY}_{ci.DisplayName}";
        //}
        void SetCurrentCultureToCache(CultureInfo ci) => currentCulture = ci.Name;
        protected bool IsUICultureCurrentCulture(CultureInfo ci) {
            return string.Equals(currentCulture, ci.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        protected void GetCultureToUse(CultureInfo cultureToUse)
        {
            if (!_memCache.TryGetValue(GetCacheKey(cultureToUse), out localization))
            {
                if (_memCache.TryGetValue(GetCacheKey(cultureToUse.Parent), out localization))
                {
                    SetCurrentCultureToCache(cultureToUse.Parent);
                }
                else
                {
                    _memCache.TryGetValue(GetCacheKey(cultureToUse), out localization);
                    SetCurrentCultureToCache(_localizationOptions.Value.DefaultCulture);
                }
            }
            SetCurrentCultureToCache(cultureToUse);
        }

        protected void InitJsonStringLocalizer()
        {
            AddMissingCultureToSupportedCulture(CultureInfo.CurrentUICulture);
            AddMissingCultureToSupportedCulture(_localizationOptions.Value.DefaultCulture);

            foreach (CultureInfo ci in _localizationOptions.Value.SupportedCultureInfos)
            {
                InitJsonStringLocalizer(ci);
            }

            //after initialization, get current ui culture
            GetCultureToUse(CultureInfo.CurrentUICulture);
        }

        protected void AddMissingCultureToSupportedCulture(CultureInfo cultureInfo)
        {
            if (!_localizationOptions.Value.SupportedCultureInfos.Contains(cultureInfo))
            {
                _localizationOptions.Value.SupportedCultureInfos.Add(cultureInfo);
            }
        }

        protected void InitJsonStringLocalizer(CultureInfo currentCulture)
        {
            //Look for cache key.
            if (!_memCache.TryGetValue(GetCacheKey(currentCulture), out localization))
            {
                ConstructLocalizationObject(resourcesRelativePath, currentCulture);
                // Set cache options.
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(_memCacheDuration);

                // Save data in cache.
                _memCache.Set(GetCacheKey(currentCulture), localization, cacheEntryOptions);
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

            string basePath = string.IsNullOrWhiteSpace(_baseName) ? jsonPath : Path.Combine(jsonPath, _baseName);
            if (!Directory.Exists(basePath)) return;
            string pattern = "*.json";

            //get all files ending by json extension
            string[] myFiles = Directory.GetFiles(basePath, pattern, SearchOption.AllDirectories);

            foreach (string file in myFiles)
            {
                Dictionary<string, JsonLocalizationFormat> tempLocalization = JsonConvert.DeserializeObject<Dictionary<string, JsonLocalizationFormat>>(File.ReadAllText(file, _localizationOptions.Value.FileEncoding));
                foreach (KeyValuePair<string, JsonLocalizationFormat> temp in tempLocalization)
                {
                    LocalizatedFormat localizedValue = GetLocalizedValue(currentCulture, temp);
                    if (!(localizedValue.Value is null))
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
            string value = temp.Value.Values.FirstOrDefault(s => string.Equals(s.Key, currentCulture.Name, StringComparison.InvariantCultureIgnoreCase)).Value;
            if (value is null)
            {
                isParent = true;
                value = temp.Value.Values.FirstOrDefault(s => string.Equals(s.Key, currentCulture.Parent.Name, StringComparison.InvariantCultureIgnoreCase)).Value;
                if (value is null)
                {
                    value = temp.Value.Values.FirstOrDefault(s => string.IsNullOrWhiteSpace(s.Key)).Value;
                    if (value is null && _localizationOptions.Value.DefaultCulture != null)
                    {
                        value = temp.Value.Values.FirstOrDefault(s => string.Equals(s.Key, _localizationOptions.Value.DefaultCulture.Name, StringComparison.InvariantCultureIgnoreCase)).Value;
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
            if (!string.IsNullOrEmpty(baseName))
            {
                string friendlyName = string.Empty;

                friendlyName = AppDomain.CurrentDomain.FriendlyName;

                //return baseName.Replace($"{friendlyName}.", "").Replace(".", "/");
                return baseName.Replace($"{friendlyName}.", "").Replace(".", Path.DirectorySeparatorChar.ToString());
            }
            return null;
        }



    }
}