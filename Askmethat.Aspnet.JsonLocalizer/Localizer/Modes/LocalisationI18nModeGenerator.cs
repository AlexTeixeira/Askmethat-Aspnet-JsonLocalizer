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
    internal class LocalisationI18nModeGenerator : ILocalisationModeGenerator
    {
        private ConcurrentDictionary<string, LocalizatedFormat> localization =
            new ConcurrentDictionary<string, LocalizatedFormat>();

        private JsonLocalizationOptions _options;

        public ConcurrentDictionary<string, LocalizatedFormat> ConstructLocalization(IEnumerable<string> myFiles,
            CultureInfo currentCulture,
            JsonLocalizationOptions options)
        {
            _options = options;

            var neutralFile = myFiles.FirstOrDefault(file => file.Split("/")
                .Last().Count(s => s.CompareTo('.') == 0) == 1);

            var cultureExistInFiles = myFiles.Any(file => file.Split("/").Any(
                s => s.Contains(currentCulture.Name, StringComparison.OrdinalIgnoreCase)
                     || s.Contains(currentCulture.Parent.Name, StringComparison.OrdinalIgnoreCase)
            )) && currentCulture.DisplayName != CultureInfo.InvariantCulture.ThreeLetterISOLanguageName;

            if (cultureExistInFiles)
            {
                foreach (string file in myFiles)
                {
                    var splittedFiles = file.Split("/");
                    var fileCulture = new CultureInfo(splittedFiles[^1].Split(".")[1]);

                    var isParent =
                        fileCulture.Name.Equals(currentCulture.Parent.Name, StringComparison.OrdinalIgnoreCase);

                    if (fileCulture.Name.Equals(currentCulture.Name, StringComparison.OrdinalIgnoreCase) ||
                        isParent && fileCulture.Name != "json")
                    {
                        AddValueToLocalization(options, file, isParent);
                    }
                }
            }
            else
            {
                AddValueToLocalization(options, neutralFile, true);
            }
            
            return localization;
        }

        private void AddValueToLocalization(JsonLocalizationOptions options, string file, bool isParent)
        {
            ConcurrentDictionary<string, string> tempLocalization =
                LocalisationModeHelpers.ReadAndDeserializeFile<string, string>(file, options.FileEncoding);

            if (tempLocalization == null)
            {
                return;
            }

            foreach (var temp in tempLocalization)
            {
                LocalizatedFormat localizedValue = GetLocalizedValue(temp, isParent);

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

        private LocalizatedFormat GetLocalizedValue(KeyValuePair<string, string> temp, bool isParent)
        {
            return new LocalizatedFormat()
            {
                IsParent = isParent,
                Value = temp.Value
            };
        }
    }
}