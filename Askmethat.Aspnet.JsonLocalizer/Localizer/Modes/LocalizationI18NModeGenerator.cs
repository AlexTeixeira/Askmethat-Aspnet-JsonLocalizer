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
    internal class LocalizationI18NModeGenerator : LocalizationModeBase, ILocalizationModeGenerator
    {

        private LocalizatedFormat GetLocalizedValue(KeyValuePair<string, string> temp, bool isParent)
        {
            return new LocalizatedFormat()
            {
                IsParent = isParent,
                Value = temp.Value as string
            };
        }
        
        public ConcurrentDictionary<string, LocalizatedFormat> ConstructLocalization(IEnumerable<string> myFiles,
            CultureInfo currentCulture,
            JsonLocalizationOptions options)
        {
            _options = options;

            var neutralFile = myFiles.FirstOrDefault(file => file.Split("/")
                .Last().Count(s => s.CompareTo('.') == 0) == 1);

            var isNotInvariantCulture =
                currentCulture.DisplayName != CultureInfo.InvariantCulture.ThreeLetterISOLanguageName;

            var files = myFiles.Where(file => file.Split("/").Any(
                s => s.Contains(currentCulture.Name, StringComparison.OrdinalIgnoreCase)
                     || s.Contains(currentCulture.Parent.Name, StringComparison.OrdinalIgnoreCase)
            )).ToArray();

            if (files.Any() && isNotInvariantCulture)
            {
                foreach (var file in files)
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

                AddOrUpdateLocalizedValue(localizedValue, temp);
            }
        }

    }
}