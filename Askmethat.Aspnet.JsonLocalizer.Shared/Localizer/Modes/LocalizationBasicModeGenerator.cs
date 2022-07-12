using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Askmethat.Aspnet.JsonLocalizer.Format;
using Askmethat.Aspnet.JsonLocalizer.JsonOptions;

namespace Askmethat.Aspnet.JsonLocalizer.Localizer.Modes
{
    internal class LocalizationBasicModeGenerator : LocalizationModeBase, ILocalizationModeGenerator
    {
        public ConcurrentDictionary<string, LocalizatedFormat> ConstructLocalization(
            IEnumerable<string> myFiles, CultureInfo currentCulture, JsonLocalizationOptions options)
        {
            _options = options;
            
            foreach (string file in myFiles)
            {
                ConcurrentDictionary<string, JsonLocalizationFormat> tempLocalization = null;
                try
                {
                    tempLocalization =
                        LocalisationModeHelpers.ReadAndDeserializeFile<string, JsonLocalizationFormat>(file,
                            options.FileEncoding);
                }
                catch (Exception ex)
                {
                    if (!options.IgnoreJsonErrors)
                        throw;
                        
                }

                if (tempLocalization == null)
                {
                    continue;
                }

                foreach (KeyValuePair<string, JsonLocalizationFormat> temp in tempLocalization)
                {
                    LocalizatedFormat localizedValue = GetLocalizedValue(currentCulture, temp);
                    AddOrUpdateLocalizedValue<JsonLocalizationFormat>(localizedValue, temp);
                }
            }

            return localization;
        }

        private LocalizatedFormat GetLocalizedValue(CultureInfo currentCulture,
            KeyValuePair<string, JsonLocalizationFormat> temp)
        {
            var localizationFormat = temp.Value;
            bool isParent = false;
            string value = localizationFormat.Values.FirstOrDefault(s =>
                string.Equals(s.Key, currentCulture.Name, StringComparison.OrdinalIgnoreCase)).Value;
            if (value is null)
            {
                isParent = true;
                value = localizationFormat.Values.FirstOrDefault(s =>
                    string.Equals(s.Key, currentCulture.Parent.Name, StringComparison.OrdinalIgnoreCase)).Value;
                if (value is null)
                {
                    value = localizationFormat.Values.FirstOrDefault(s => string.IsNullOrWhiteSpace(s.Key)).Value;
                    if (value is null && _options.DefaultCulture != null)
                    {
                        value = localizationFormat.Values.FirstOrDefault(s => string.Equals(s.Key,
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