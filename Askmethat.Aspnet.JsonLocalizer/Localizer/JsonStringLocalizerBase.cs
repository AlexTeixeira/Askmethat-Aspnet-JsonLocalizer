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
            _baseName = CleanBaseName(baseName);
            _localizationOptions = localizationOptions;
            _memCache = _localizationOptions.Value.Caching;
            _memCacheDuration = _localizationOptions.Value.CacheDuration;
        }

        private string GetCacheKey(CultureInfo ci) => $"{CACHE_KEY}_{ci.DisplayName}";

        //string GetCacheKey(CultureInfo ci)
        //{
        //    if (_localizationOptions.Value.UseBaseName)
        //    {
        //        return $"{CACHE_KEY}_{ci.DisplayName}_{_baseName}";
        //    }
        //    return $"{CACHE_KEY}_{ci.DisplayName}";
        //}
        private void SetCurrentCultureToCache(CultureInfo ci) => currentCulture = ci.Name;
        protected bool IsUICultureCurrentCulture(CultureInfo ci)
        {
            return string.Equals(currentCulture, ci.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        protected void GetCultureToUse(CultureInfo cultureToUse)
        {
            if (_memCache.TryGetValue(GetCacheKey(cultureToUse), out localization))
            {
                SetCurrentCultureToCache(cultureToUse);
                return;
            }

            if (_memCache.TryGetValue(GetCacheKey(cultureToUse.Parent), out localization))
            {
                SetCurrentCultureToCache(cultureToUse.Parent);
                return;
            }

            if (_memCache.TryGetValue(GetCacheKey(_localizationOptions.Value.DefaultCulture), out localization))
            {
                SetCurrentCultureToCache(_localizationOptions.Value.DefaultCulture);
            }
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
                _ = _localizationOptions.Value.SupportedCultureInfos.Add(cultureInfo);
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
                _ = _memCache.Set(GetCacheKey(currentCulture), localization, cacheEntryOptions);
            }
        }

        /// <summary>
        /// Construct localization object from json files
        /// </summary>
        /// <param name="jsonPath">Json file path</param>
        private void ConstructLocalizationObject(string jsonPath, CultureInfo currentCulture)
        {
            //be sure that localization is always initialized
            if (localization == null)
            {
                localization = new Dictionary<string, LocalizatedFormat>();
            }

            IEnumerable<string> myFiles = GetMatchingJsonFiles(jsonPath);

            foreach (string file in myFiles)
            {
                Dictionary<string, JsonLocalizationFormat> tempLocalization = JsonConvert.DeserializeObject<Dictionary<string, JsonLocalizationFormat>>(File.ReadAllText(file, _localizationOptions.Value.FileEncoding));
                if (tempLocalization == null)
                {
                    continue;
                }
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

        private IEnumerable<string> GetMatchingJsonFiles(string jsonPath)
        {
            string searchPattern = "*.json";
            SearchOption searchOption = SearchOption.AllDirectories;
            string basePath = jsonPath;
            const string sharedSearchPattern = "*.shared.json";
            List<string> files = new List<string>();
            if (_localizationOptions.Value.UseBaseName && !string.IsNullOrWhiteSpace(_baseName))
            {
                /*
                 https://docs.microsoft.com/de-de/aspnet/core/fundamentals/localization?view=aspnetcore-2.2#dataannotations-localization
                    Using the option ResourcesPath = "Resources", the error messages in RegisterViewModel can be stored in either of the following paths:
                    Resources/ViewModels.Account.RegisterViewModel.fr.resx
                    Resources/ViewModels/Account/RegisterViewModel.fr.resx
                 */

                searchOption = SearchOption.TopDirectoryOnly;
                string friendlyName = AppDomain.CurrentDomain.FriendlyName;

                string shortName = _baseName.Replace($"{friendlyName}.", "");

                basePath = Path.Combine(jsonPath, TransformNameToPath(shortName));
                if (Directory.Exists(basePath))
                {
                    // We can search something like Resources/ViewModels/Account/RegisterViewModel/*.json
                    searchPattern = "*.json";
                }
                else
                {  // We search something like Resources/ViewModels/Account/RegisterViewModel.json
                    int lastDot = shortName.LastIndexOf('.');
                    string className = shortName.Substring(lastDot + 1);
                    // Remove class name from shortName so we can use it as folder.
                    string baseFolder = shortName.Substring(0, lastDot);
                    baseFolder = TransformNameToPath(baseFolder);

                    basePath = Path.Combine(jsonPath, baseFolder);

                    if (Directory.Exists(basePath))
                    {
                        searchPattern = $"{className}?.json";
                    }
                    else
                    { // We search something like Resources/ViewModels.Account.RegisterViewModel.json
                        basePath = jsonPath;
                        searchPattern = $"{shortName}?.json";
                    }
                }

                files = Directory.GetFiles(basePath, searchPattern, searchOption).ToList();
                //add sharedfile that should be found in base path
                files.AddRange(Directory.GetFiles(basePath, sharedSearchPattern, SearchOption.TopDirectoryOnly));
            }
            else
            {
                files = Directory.GetFiles(basePath, searchPattern, searchOption).ToList();
            }

            // Get all files ending by json extension
            return files;
        }



        private string TransformNameToPath(string name)
        {
            return !string.IsNullOrEmpty(name) ? name.Replace(".", Path.DirectorySeparatorChar.ToString()) : null;
        }

        private string CleanBaseName(string baseName)
        {
            if (!string.IsNullOrEmpty(baseName))
            {
                // Nested classes are seperated by + and should use the translation of their parent class.
                int plusIdx = baseName.IndexOf('+');
                return plusIdx == -1 ? baseName : baseName.Substring(0, plusIdx);
            }
            else
            {
                return string.Empty;
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

    }
}