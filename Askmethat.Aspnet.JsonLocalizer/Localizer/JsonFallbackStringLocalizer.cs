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
    internal class JsonFallbackStringLocalizer : JsonStringLocalizerBase, IStringLocalizer
    {

        public JsonFallbackStringLocalizer(IHostingEnvironment env, IMemoryCache memCache, string resourcesRelativePath, IOptions<JsonLocalizationOptions> localizationOptions) : base(env, memCache, resourcesRelativePath, localizationOptions)
        {
        }

        public JsonFallbackStringLocalizer(IHostingEnvironment env, IMemoryCache memCache, IOptions<JsonLocalizationOptions> localizationOptions) : base(env, memCache, localizationOptions)
        {
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
                    .Where(l => l.Values.ContainsKey(CultureInfo.CurrentCulture.Name))
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
