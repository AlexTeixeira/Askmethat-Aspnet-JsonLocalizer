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
    internal class JsonStringLocalizer : JsonStringLocalizerBase, IStringLocalizer
    {

        public JsonStringLocalizer(string resourcesRelativePath, IOptions<JsonLocalizationOptions> localizationOptions, string baseName = "") : base(resourcesRelativePath, localizationOptions, baseName)
        {
        }

        public JsonStringLocalizer(IOptions<JsonLocalizationOptions> localizationOptions) : base(localizationOptions)
        {
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
                var value = GetPluralLocalization(name, format, arguments);
                return new LocalizedString(name, value, resourceNotFound: format == null);
            }
        }

        private string GetPluralLocalization(string name, string format, object[] arguments)
        {
            var last = arguments.LastOrDefault();
            string value = string.Empty;
            if (last != null && last is bool)
            {
                bool isPlural = (bool)last;
                value = GetString(name);
                int index = (isPlural ? 1 : 0);
                value = value.Split(_localizationOptions.Value.PluralSeparator)[index];
            }
            else
            {
                value = String.Format(format ?? name, arguments);
            }

            return value;
        }
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return includeParentCultures
                ? localization
                    .Select(
                        l =>
                        {
                            var value = GetString(l.Key);
                            return new LocalizedString(l.Key, value ?? l.Key, resourceNotFound: value == null);
                        }
                    )
                : localization
                    .Where(l => l.Value.Values.ContainsKey(CultureInfo.CurrentUICulture.LCID))
                    .Select(l => new LocalizedString(l.Key, l.Value.Values[CultureInfo.CurrentUICulture.LCID], false));
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return new JsonStringLocalizer(_resourcesRelativePath, _localizationOptions);
        }

        string GetString(string name, CultureInfo cultureInfo = null, bool shouldTryDefaultCulture = true)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (cultureInfo == null)
            {
                cultureInfo = CultureInfo.CurrentUICulture;
            }

            LocalizationFormat keyObject = null;

            if (localization.TryGetValue(name, out keyObject))
            {
                var localizedValue = string.Empty;
                if (keyObject.Values.TryGetValue(cultureInfo.LCID, out localizedValue))
                {
                    return localizedValue;
                }
            }

            if (!cultureInfo.Equals(_localizationOptions.Value.DefaultCulture) && !cultureInfo.Equals(cultureInfo.Parent))
            {
                Console.Error.WriteLine($"{name} is using parent culture instead of current ui culture");
                //Try the parent culture
                return GetString(name, cultureInfo.Parent, shouldTryDefaultCulture);
            }

            if (shouldTryDefaultCulture && !cultureInfo.Equals(_localizationOptions.Value.DefaultCulture))
            {
                Console.Error.WriteLine($"{name} is using default option culture instead of current ui culture");
                //Try the default culture
                return GetString(name, _localizationOptions.Value.DefaultCulture, false);
            }

            //advert user that current name string does not 
            //contains any translation
            Console.Error.WriteLine($"{name} does not contains any translation");
            return null;
        }
    }
}
