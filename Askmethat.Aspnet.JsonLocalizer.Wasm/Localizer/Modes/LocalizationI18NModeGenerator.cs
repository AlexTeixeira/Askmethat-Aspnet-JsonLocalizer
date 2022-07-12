using Askmethat.Aspnet.JsonLocalizer.JsonOptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Askmethat.Aspnet.JsonLocalizer.Localizer.Modes
{
    internal partial class LocalizationI18NModeGenerator
    {
        private static string ReadFile(JsonLocalizationOptions options, string file)
        {
            var thisAssembly = Assembly.GetEntryAssembly();
            var assemblyResource = file.Replace('/', '.').Replace('\\', '.');
            using var stream = thisAssembly.GetManifestResourceStream(assemblyResource);
            if (stream is null) throw new InvalidProgramException($"Resource '{assemblyResource}' cannot be found in '{thisAssembly.FullName}'");
            using var reader = new StreamReader(stream, options.FileEncoding);
            return reader.ReadToEnd();
            //the code should call this but I don't think it is possible to get into an async context
            //return await Http.GetStringAsync(file);
            //return File.ReadAllText(file, options.FileEncoding);
        }
    }
}
