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

        public JsonStringLocalizer(IHostingEnvironment env, IMemoryCache memCache, string resourcesRelativePath, IOptions<JsonLocalizationOptions> localizationOptions) : base(env, memCache, resourcesRelativePath, localizationOptions)
        {
        }

        public JsonStringLocalizer(IHostingEnvironment env, IMemoryCache memCache, IOptions<JsonLocalizationOptions> localizationOptions) : base(env, memCache, localizationOptions)
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
                var value = String.Format(format ?? name, arguments);
                return new LocalizedString(name, value, resourceNotFound: format == null);
            }
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
                    .Where(l => l.Values.ContainsKey(CultureInfo.CurrentUICulture.Name))
                    .Select(l => new LocalizedString(l.Key, l.Values[CultureInfo.CurrentUICulture.Name], false));
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return new JsonStringLocalizer(_env, _memCache, _resourcesRelativePath, _localizationOptions);
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

            var valuesSection = localization
                .Where(l => l.Values.ContainsKey(cultureInfo.Name))
                .FirstOrDefault(l => l.Key == name);

            if (valuesSection != null)
            {
                return valuesSection.Values[cultureInfo.Name];
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
