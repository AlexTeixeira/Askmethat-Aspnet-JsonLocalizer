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

            var neutralFile = myFiles.FirstOrDefault(file => file.Split(Path.AltDirectorySeparatorChar)
                .Last().Count(s => s.CompareTo('.') == 0) == 1);

            var isInvariantCulture =
                currentCulture.DisplayName == CultureInfo.InvariantCulture.ThreeLetterISOLanguageName;

            var files = isInvariantCulture ? new string[]{} : myFiles.Where(file => file.Split(Path.AltDirectorySeparatorChar).Any(
                s => (s.IndexOf(currentCulture.Name, StringComparison.OrdinalIgnoreCase) >= 0
                      || s.IndexOf(currentCulture.Parent.Name, StringComparison.OrdinalIgnoreCase) >= 0)
            )).ToArray(); 
            

            if (files.Any() && !isInvariantCulture)
            {
                foreach (var file in files)
                {
                    var splittedFiles = file.Split(Path.AltDirectorySeparatorChar);
                    var stringCulture = splittedFiles.Last().Split('.')[1];
                    var fileCulture = new CultureInfo(stringCulture);

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