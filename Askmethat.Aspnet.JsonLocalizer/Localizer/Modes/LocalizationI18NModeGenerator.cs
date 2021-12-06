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

        private void AddValueToLocalization(JsonLocalizationOptions options, string file, bool isParent)
        {
            var json = Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText(file, options.FileEncoding));

            if (json == null)
            {
                return;
            }

            AddValues(json, null, isParent);
        }

        private void AddValues(dynamic input, string baseName, bool isParent)
        {
            // Json Object could either contain an array or an object or just values
            // For the field names, navigate to the root or the first element
            input = input.Root ?? input.First ?? input;

            if (input != null)
            {
                // check if the object is of type JObject. 
                // If yes, read the properties of that JObject
                if (input.GetType() == typeof(Newtonsoft.Json.Linq.JObject))
                {
                    // Create JObject from object
                    Newtonsoft.Json.Linq.JObject inputJson =
                        Newtonsoft.Json.Linq.JObject.FromObject(input);

                    // Read Properties
                    var properties = inputJson.Properties();

                    // Loop through all the properties of that JObject
                    foreach (var property in properties)
                    {
                        // Check if there are any sub-fields (nested)
                        if (property.Value.GetType() == typeof(Newtonsoft.Json.Linq.JObject))
                        {
                            // If yes, enter the recursive loop to extract sub-field names
                            var newBaseName = String.IsNullOrEmpty(baseName)
                                ? property.Name
                                : String.Format("{0}.{1}", baseName, property.Name);
                            AddValues(property.Value, newBaseName, isParent);
                        }
                        else if (property.Value.GetType() == typeof(Newtonsoft.Json.Linq.JArray))
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
                else
                {
                    throw new ArgumentException("Invalid i18n Json");
                }
            }
        }
    }
}