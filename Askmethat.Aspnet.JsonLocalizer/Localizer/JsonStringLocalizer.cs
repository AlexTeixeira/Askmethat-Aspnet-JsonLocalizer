using Askmethat.Aspnet.JsonLocalizer.Extensions;
using Askmethat.Aspnet.JsonLocalizer.Format;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Askmethat.Aspnet.JsonLocalizer.Localizer
{
    internal class JsonStringLocalizer : JsonStringLocalizerBase, IJsonStringLocalizer
    {
        private readonly IWebHostEnvironment _env;

        public JsonStringLocalizer(IOptions<JsonLocalizationOptions> localizationOptions, IWebHostEnvironment env, string baseName = null) : base(localizationOptions, baseName)
        {
            _env = env;
            resourcesRelativePath = GetJsonRelativePath(_localizationOptions.Value.ResourcesPath);
        }

        public LocalizedString this[string name]
        {
            get
            {
                string value = GetString(name);
                return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                string format = GetString(name);
                string value = GetPluralLocalization(name, format, arguments);
                return new LocalizedString(name, value, resourceNotFound: format == null);
            }
        }

        private string GetPluralLocalization(string name, string format, object[] arguments)
        {
            object last = arguments.LastOrDefault();
            string value;
            if (last != null && last is bool boolean)
            {
                bool isPlural = boolean;
                value = GetString(name);
                if (!string.IsNullOrEmpty(value) && value.Contains(_localizationOptions.Value.PluralSeparator))
                {
                    int index = isPlural ? 1 : 0;
                    value = value.Split(_localizationOptions.Value.PluralSeparator)[index];
                }
                else
                {
                    value = string.Format(format ?? name, arguments);
                }
            }
            else
            {
                value = string.Format(format ?? name, arguments);
            }

            return value;
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            InitJsonFromCulture(CultureInfo.CurrentUICulture); 

            return includeParentCultures ? localization?
                    .Select(
                        l =>
                        {
                            string value = GetString(l.Key);
                            return new LocalizedString(l.Key, value ?? l.Key, resourceNotFound: value == null);
                        }
                    ) :
                    localization?
                    .Where(w => !w.Value.IsParent)
                    .Select(
                        l =>
                        {
                            string value = GetString(l.Key);
                            return new LocalizedString(l.Key, value ?? l.Key, resourceNotFound: value == null);
                        }
                    ).OrderBy(s => s.Name);
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            if (!_localizationOptions.Value.SupportedCultureInfos.Contains(culture))
            {
                _localizationOptions.Value.SupportedCultureInfos.Add(culture);
            }

            CultureInfo.CurrentCulture = culture;
            
            return new JsonStringLocalizer(_localizationOptions, _env);
        }

        private string GetString(string name, bool shouldTryDefaultCulture = true)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (shouldTryDefaultCulture && !IsUICultureCurrentCulture(CultureInfo.CurrentUICulture))
            {
                InitJsonFromCulture(CultureInfo.CurrentUICulture);
            }

            if (localization != null && localization.TryGetValue(name, out LocalizatedFormat localizedValue))
            {
                return localizedValue.Value;
            }

            if (shouldTryDefaultCulture)
            {
                InitJsonFromCulture(_localizationOptions.Value.DefaultCulture);
                return GetString(name, false);
            }

            // Notify the user that a translation was not found for the current string
            // only if logging is defined in options.MissingTranslationLogBehavior
            if (_localizationOptions.Value.MissingTranslationLogBehavior ==
                MissingTranslationLogBehavior.LogConsoleError)
            {
                Console.Error.WriteLine($"{name} does not contain any translation");
            }

            return null;
        }

        private void InitJsonFromCulture(CultureInfo cultureInfo)
        {
            InitJsonStringLocalizer(cultureInfo);
            AddMissingCultureToSupportedCulture(cultureInfo);
            GetCultureToUse(cultureInfo);
        }

        /// <summary>
        /// Get path of json
        /// </summary>
        /// <returns>JSON relative path</returns>
        private string GetJsonRelativePath(string path)
        {
            string fullPath = string.Empty;
            if (_localizationOptions.Value.IsAbsolutePath)
            {
                fullPath = path;
            }
            if (!_localizationOptions.Value.IsAbsolutePath && string.IsNullOrEmpty(path))
            {
                fullPath = Path.Combine(_env.ContentRootPath, "Resources");
            }
            else if (!_localizationOptions.Value.IsAbsolutePath && !string.IsNullOrEmpty(path))
            {
                fullPath = Path.Combine(AppContext.BaseDirectory, path2: path);
            }
            return fullPath;
        }

        /// <summary>
        /// In order to use this method, JsonLocalizationOptions.ResourcesPath & JsonLocalizationOptions.IsAbsolutePath = true must be set. For more information, see: [https://github.com/AlexTeixeira/Askmethat-Aspnet-JsonLocalizer/wiki/How-file-path-works]
        /// </summary>
        /// <param name="culturesToClearFromCache">Specific cultures to clear from cache. If not provided, all cultures will be purged from cache.</param>
        public void ClearMemCache(IEnumerable<CultureInfo> culturesToClearFromCache = null)
        {
            // If one or more cultures are provided, clear only requested cultures, else clear all supported cultures.
            foreach (var cultureInfo in culturesToClearFromCache ??
                                         _localizationOptions.Value.SupportedCultureInfos.ToArray())
            {
                _memCache.Remove(GetCacheKey(cultureInfo));
            }
        }

        /// <summary>
        /// Reload memory cache
        /// </summary>
        /// <param name="reloadCulturesToCache">Reload specified cultures</param>

        public void ReloadMemCache(IEnumerable<CultureInfo> reloadCulturesToCache = null)
        {
	        ClearMemCache();
	        foreach (var cultureInfo in reloadCulturesToCache ??
	                                    _localizationOptions.Value.SupportedCultureInfos.ToArray())
	        {
                InitJsonFromCulture(cultureInfo); 
            }
        }
    }
}