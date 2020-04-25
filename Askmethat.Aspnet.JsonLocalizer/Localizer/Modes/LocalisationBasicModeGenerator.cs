using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Askmethat.Aspnet.JsonLocalizer.Format;
using Askmethat.Aspnet.JsonLocalizer.JsonOptions;
using Newtonsoft.Json;

namespace Askmethat.Aspnet.JsonLocalizer.Localizer.Modes
{
    internal class LocalisationBasicModeGenerator : ILocalisationModeGenerator
    {
        private ConcurrentDictionary<string, LocalizatedFormat> localization =
            new ConcurrentDictionary<string, LocalizatedFormat>();

        private JsonLocalizationOptions _options;
        
        public ConcurrentDictionary<string, LocalizatedFormat> ConstructLocalization(
            IEnumerable<string> myFiles, CultureInfo currentCulture, JsonLocalizationOptions options)
        {
            _options = options;
            
            foreach (string file in myFiles)
            {
                ConcurrentDictionary<string, JsonLocalizationFormat> tempLocalization = LocalisationModeHelpers.ReadAndDeserializeFile<string,JsonLocalizationFormat>(file, options.FileEncoding);

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
                            localization.TryAdd(temp.Key, localizedValue);
                        }
                        else if (localization[temp.Key].IsParent)
                        {
                            localization[temp.Key] = localizedValue;
                        }
                    }
                }
            }

            return localization;
        }

        private LocalizatedFormat GetLocalizedValue(CultureInfo currentCulture,
            KeyValuePair<string, JsonLocalizationFormat> temp)
        {
            bool isParent = false;
            string value = temp.Value.Values.FirstOrDefault(s =>
                string.Equals(s.Key, currentCulture.Name, StringComparison.OrdinalIgnoreCase)).Value;
            if (value is null)
            {
                isParent = true;
                value = temp.Value.Values.FirstOrDefault(s =>
                    string.Equals(s.Key, currentCulture.Parent.Name, StringComparison.OrdinalIgnoreCase)).Value;
                if (value is null)
                {
                    value = temp.Value.Values.FirstOrDefault(s => string.IsNullOrWhiteSpace(s.Key)).Value;
                    if (value is null && _options.DefaultCulture != null)
                    {
                        value = temp.Value.Values.FirstOrDefault(s => string.Equals(s.Key,
                            _options.DefaultCulture.Name, StringComparison.OrdinalIgnoreCase)).Value;
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