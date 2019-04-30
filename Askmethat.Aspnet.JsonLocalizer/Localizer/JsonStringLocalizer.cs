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
            string value = string.Empty;
            if (last != null && last is bool)
            {
                bool isPlural = (bool)last;
                value = GetString(name);
                if (value.Contains(_localizationOptions.Value.PluralSeparator))
                {
                    int index = (isPlural ? 1 : 0);
                    value = value.Split(_localizationOptions.Value.PluralSeparator)[index];
                }
                else
                {
                    value = String.Format(format ?? name, arguments);
                }
            }
            else
            {
                value = String.Format(format ?? name, arguments);
            }

            return value;
        }
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return includeParentCultures ? localization
                    .Select(
                        l =>
                        {
                            string value = GetString(l.Key);
                            return new LocalizedString(l.Key, value ?? l.Key, resourceNotFound: value == null);
                        }
                    ) : 
                    localization
                    .Where(w => !w.Value.IsParent)
                    .Select(
                        l =>
                        {
                            string value = GetString(l.Key);
                            return new LocalizedString(l.Key, value ?? l.Key, resourceNotFound: value == null);
                        }
                    ) 
                    ;
                
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return new JsonStringLocalizer(_resourcesRelativePath, _localizationOptions);
        }

        string GetString(string name, bool shouldTryDefaultCulture = true)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            LocalizatedFormat localizedValue = null;

            if (localization.TryGetValue(name, out localizedValue))
            {
                return localizedValue.Value; 
            }

            //advert user that current name string does not 
            //contains any translation
            Console.Error.WriteLine($"{name} does not contains any translation");
            return null;
        }
    }
}
