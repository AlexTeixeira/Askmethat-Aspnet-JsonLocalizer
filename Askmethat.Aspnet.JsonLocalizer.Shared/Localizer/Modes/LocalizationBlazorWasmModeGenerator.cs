using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Askmethat.Aspnet.JsonLocalizer.Format;
using Askmethat.Aspnet.JsonLocalizer.JsonOptions;


namespace Askmethat.Aspnet.JsonLocalizer.Localizer.Modes
{
    internal class LocalizationBlazorWasmModeGenerator : LocalizationI18NModeGenerator
    {
        private readonly Assembly resourceAssembly;

        public LocalizationBlazorWasmModeGenerator(Assembly resourceAssembly)
        {
            this.resourceAssembly = resourceAssembly;
        }

        public new ConcurrentDictionary<string, LocalizatedFormat> ConstructLocalization(IEnumerable<string> myFiles,
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

        public static T ExecuteSynchronously<T>(Func<Task<T>> taskFunc)
        {
            var capturedContext = SynchronizationContext.Current;
            try
            {
                SynchronizationContext.SetSynchronizationContext(null);
                return taskFunc.Invoke().GetAwaiter().GetResult();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(capturedContext);
            }
        }
    }
}