using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Askmethat.Aspnet.JsonLocalizer.Format;
using Askmethat.Aspnet.JsonLocalizer.JsonOptions;

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

            var enumerable = myFiles as string[] ?? myFiles.ToArray();
            var neutralFiles = enumerable.Where(file => Path.GetFileName(file)
                .Count(s => s.CompareTo('.') == 0) == 1).ToList();
            var isInvariantCulture =
                currentCulture.DisplayName == CultureInfo.InvariantCulture.ThreeLetterISOLanguageName;

            var files = isInvariantCulture
                ? new string[] { }
                : enumerable.Where(file => Path.GetFileName(file).Split('.').Any(
                    s => (s.IndexOf(currentCulture.Name, StringComparison.OrdinalIgnoreCase) >= 0
                          || s.IndexOf(currentCulture.Parent.Name, StringComparison.OrdinalIgnoreCase) >= 0)
                )).ToArray();


            if (files.Any() && !isInvariantCulture)
            {
                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file);
                    var fileCulture = new CultureInfo(fileName.Split('.')[^2] ?? String.Empty);

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
                if (neutralFiles.Any())
                {
                    foreach (var neutralFile in neutralFiles)
                        AddValueToLocalization(options, neutralFile, true);
                }
            }

            return localization;
        }

        internal void AddValueToLocalization(JsonLocalizationOptions options, string file, bool isParent)
        {
            using var doc = JsonDocument.Parse(File.ReadAllText(file, options.FileEncoding));
            if (doc is null)
            {
                return;
            }

            AddValues(doc.RootElement, null, isParent);
        }

        internal void AddValues(JsonElement element, string baseName, bool isParent)
        {
            // Json Object could either contain an array or an object or just values
            // For the field names, navigate to the root or the first element
            var input = element;


            // check if the object is of type JObject. 
            // If yes, read the properties of that JObject
            if (input.ValueKind == JsonValueKind.Object)
            {
                // Read Properties
                var properties = input.EnumerateObject();

                // Loop through all the properties of that JObject
                foreach (var property in properties)
                {
                    // Check if there are any sub-fields (nested)
                    if (property.Value.ValueKind == JsonValueKind.Object)
                    {
                        // If yes, enter the recursive loop to extract sub-field names
                        var newBaseName = String.IsNullOrEmpty(baseName)
                            ? property.Name
                            : String.Format("{0}.{1}", baseName, property.Name);
                        AddValues(property.Value, newBaseName, isParent);
                    }
                    else if (property.Value.ValueKind == JsonValueKind.Array)
                    {
                        throw new ArgumentException("Invalid i18n Json");
                    }
                    else
                    {
                        // If there are no sub-fields, the property name is the field name
                        var temp = new KeyValuePair<string, string>(
                            String.IsNullOrEmpty(baseName)
                                ? property.Name
                                : $"{baseName}.{property.Name}",
                            property.Value.ToString());

                        LocalizatedFormat localizedValue = GetLocalizedValue(temp, isParent);
                        AddOrUpdateLocalizedValue(
                            localizedValue,
                            temp
                        );
                    }
                }

            }
        }
    }
}