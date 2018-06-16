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
                return new LocalizedString(name, value ?? GetString(name, _localizationOptions.Value.DefaultCulture), resourceNotFound: value == null);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var format = GetString(name);
                var value = string.Format(format ?? GetString(name, _localizationOptions.Value.DefaultCulture), arguments);
                return new LocalizedString(name, value, resourceNotFound: format == null);
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return localization.Where(l => l.Values.Keys.Any(lv => lv == CultureInfo.CurrentCulture.Name)).Select(l => new LocalizedString(l.Key, l.Values[CultureInfo.CurrentCulture.Name], true));
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return new JsonStringLocalizer(_env, _memCache, _resourcesRelativePath, _localizationOptions);
        }

        /// <summary>
        /// Get the string from JSON cached file
        /// </summary>
        /// <param name="name">Value name</param>
        /// <returns>Value if thing</returns>
        string GetString(string name)
        {
            return GetValueString(name, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Get the string from JSON cached file
        /// </summary>
        /// <param name="name">Value name</param>
        /// <returns>Value if thing</returns>
        string GetString(string name, CultureInfo cultureInfo)
        {
            return GetValueString(name, cultureInfo);
        }

        string GetValueString(string name, CultureInfo cultureInfo)
        {
            var query = localization.Where(l => l.Values.Keys.Any(lv => lv == cultureInfo.Name));
            var value = query.FirstOrDefault(l => l.Key == name);


            if (value == null && cultureInfo.Name == _localizationOptions.Value.DefaultCulture.Name)
            {
                string msg = $"Any value was found for the Key : {name}";
                Console.WriteLine(msg);
                throw new ArgumentException(msg);
            }
            else if (value == null)
            {
                return null;
            }

            return value.Values[cultureInfo.Name];
        }
    }
}
