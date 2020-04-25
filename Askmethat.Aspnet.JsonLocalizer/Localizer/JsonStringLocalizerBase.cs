using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Askmethat.Aspnet.JsonLocalizer.Caching;
using Askmethat.Aspnet.JsonLocalizer.Extensions;
using Askmethat.Aspnet.JsonLocalizer.Format;
using Askmethat.Aspnet.JsonLocalizer.JsonOptions;
using Askmethat.Aspnet.JsonLocalizer.Localizer.Modes;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Askmethat.Aspnet.JsonLocalizer.Localizer
{
    internal class JsonStringLocalizerBase
    {
        #region properties and constructor

        protected readonly CacheHelper _memCache;
        protected readonly IOptions<JsonLocalizationOptions> _localizationOptions;
        protected readonly string _baseName;
        protected readonly TimeSpan _memCacheDuration;

        protected const string CACHE_KEY = "LocalizationBlob";
        protected string resourcesRelativePath;
        protected string currentCulture = string.Empty;
        protected ConcurrentDictionary<string, LocalizatedFormat> localization;

        public JsonStringLocalizerBase(IOptions<JsonLocalizationOptions> localizationOptions, string baseName = null)
        {
            _baseName = CleanBaseName(baseName);
            _localizationOptions = localizationOptions;

            if (_localizationOptions.Value.LocalizationMode == LocalizationMode.I18n && _localizationOptions.Value.UseBaseName)
            {
                throw new ArgumentException("UseBaseName can't be activated with I18n localisation mode");
            }
            
            _memCache = _localizationOptions.Value.DistributedCache != null ?
                new CacheHelper(_localizationOptions.Value.DistributedCache) :
                new CacheHelper(_localizationOptions.Value.Caching);
            
            _memCacheDuration = _localizationOptions.Value.CacheDuration;
        }
        #endregion

        #region cache and culture methods
        protected string GetCacheKey(CultureInfo ci) => $"{CACHE_KEY}_{ci.Name}";

        private void SetCurrentCultureToCache(CultureInfo ci) => currentCulture = ci.Name;
        protected bool IsUICultureCurrentCulture(CultureInfo ci)
        {
            return string.Equals(currentCulture, ci.Name, StringComparison.OrdinalIgnoreCase);
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
        #endregion

        #region files initialization

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

                // Save data in cache.
                _memCache.Set(GetCacheKey(currentCulture), localization, _memCacheDuration);
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
                localization = new ConcurrentDictionary<string, LocalizatedFormat>();
            }

            IEnumerable<string> myFiles = GetMatchingJsonFiles(jsonPath);

            localization = LocalizationModeFactory.GetLocalisationFromMode(_localizationOptions.Value.LocalizationMode)
                .ConstructLocalization(myFiles, currentCulture, _localizationOptions.Value);
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
                    { 
                        // We search something like Resources/ViewModels.Account.RegisterViewModel.json
                        basePath = jsonPath;
                        searchPattern = $"{shortName}?.json";
                    }
                }
					
                files = Directory.GetFiles(basePath, searchPattern, searchOption).ToList();
                //add sharedfile that should be found in base path
                files.AddRange(Directory.GetFiles(basePath, sharedSearchPattern, SearchOption.TopDirectoryOnly));
                //get the base shared files
                files.AddRange(Directory.GetFiles(jsonPath, $"localization.shared.json", SearchOption.TopDirectoryOnly));
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
        #endregion
    }
}