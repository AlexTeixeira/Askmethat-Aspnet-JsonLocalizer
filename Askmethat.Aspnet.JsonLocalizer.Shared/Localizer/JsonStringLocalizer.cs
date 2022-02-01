using Askmethat.Aspnet.JsonLocalizer.Extensions;
using Askmethat.Aspnet.JsonLocalizer.Format;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Askmethat.Aspnet.JsonLocalizer.JsonOptions;
#if NETCORE
using Microsoft.AspNetCore.Components;
#endif
using Newtonsoft.Json;

namespace Askmethat.Aspnet.JsonLocalizer.Localizer
{
    internal partial class JsonStringLocalizer : JsonStringLocalizerBase, IJsonStringLocalizer
    {

        private IDictionary<string, string> _missingJsonValues = null;
        private string _missingTranslations = null;

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

        public LocalizedString GetPlural(string name, double count, params object[] arguments)
        {
            bool shouldTryDefaultCulture = true;

            if (shouldTryDefaultCulture && !IsUICultureCurrentCulture(CultureInfo.CurrentUICulture))
            {
                InitJsonFromCulture(CultureInfo.CurrentUICulture);
            }
            else if (shouldTryDefaultCulture)
            {
                InitJsonFromCulture(_localizationOptions.Value.DefaultCulture);
            }

            IPluralizationRuleSet pluralizationRuleSet = GetPluralizationToUse();

            var applicableRule = pluralizationRuleSet.GetMatchingPluralizationRule(count);
            var nameWithRule = $"{name}.{applicableRule}";

            string format = name;

            if (localization != null)
            {
                // try get the localization for the specified rule
                if (localization.TryGetValue(nameWithRule, out LocalizatedFormat localizedValue))
                {
                    format = localizedValue.Value;
                }
                else
                {
                    // if no translation was found for that rule, try with the "Other" rule.
                    var nameWithOtherRule = $"{name}.{PluralizationConstants.Other}";
                    if (localization.TryGetValue(nameWithOtherRule, out LocalizatedFormat localizedOtherValue))
                    {
                        format = localizedOtherValue.Value;
                    }
                    else // no pluralized value found. Check out if it's a normal non-pluralized translation
                    {
                        format = GetString(name, true);
                    }
                }
            }

            var argumentsWithCount = arguments.ToList();
            argumentsWithCount.Insert(0, count);

            // By this point we either found a pluralized or non pluralized translation, or we stick to the received string.
            var value = string.Format(format ?? name, argumentsWithCount.ToArray());

            return new LocalizedString(name, value, format != null);
        }
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            InitJsonFromCulture(CultureInfo.CurrentUICulture);

            return includeParentCultures
                ? localization?
                    .Select(
                        l =>
                        {
                            string value = GetString(l.Key);
                            return new LocalizedString(l.Key, value ?? l.Key, resourceNotFound: value == null);
                        }
                    )
                : localization?
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
                Console.Error.WriteLine($"'{name}' does not contain any translation");
            }

            // Notify the user that a translation was not found for the current string
            // only if logging is defined in options.MissingTranslationLogBehavior
            if (_localizationOptions.Value.MissingTranslationLogBehavior ==
                MissingTranslationLogBehavior.CollectToJSON)
            {
                if (_missingJsonValues is null)
                    _missingJsonValues = new Dictionary<string, string>();
                if (_missingJsonValues.TryAdd(name, name))
                {
                    Console.Error.WriteLine($"'{name}' added to missing values");
                    WriteMissingTranslations();
                }
            }

            return null;
        }

#if NETCORE

        public MarkupString GetHtmlBlazorString(string name, bool shouldTryDefaultCulture = true)
        {
            return new MarkupString(GetString(name, shouldTryDefaultCulture));
        }

#endif
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

        private void WriteMissingTranslations()
        {
            if (!string.IsNullOrWhiteSpace(_missingTranslations) && (_missingJsonValues?.Count ?? 0) > 0)
            {
                try
                {
                    // save missing values
                    var json = JsonConvert.SerializeObject(_missingJsonValues);
                    Console.Error.WriteLine($"Writing {_missingJsonValues?.Count} missing translations to {Path.GetFullPath(_missingTranslations)}");
                    lock (this)
                    {
                        File.WriteAllText(_missingTranslations, json);
                    }
                }
                catch (Exception)
                {
                    Console.Error.WriteLine($"Cannot write missing translations to {Path.GetFullPath(_missingTranslations)}");
                }
            }
        }

    }
}