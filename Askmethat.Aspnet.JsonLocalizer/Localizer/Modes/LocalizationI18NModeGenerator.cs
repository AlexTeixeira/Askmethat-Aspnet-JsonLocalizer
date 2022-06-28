using Askmethat.Aspnet.JsonLocalizer.JsonOptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Askmethat.Aspnet.JsonLocalizer.Localizer.Modes
{
    internal partial class LocalizationI18NModeGenerator
    {
        private static string ReadFile(JsonLocalizationOptions options, string file)
        {
            return File.ReadAllText(file, options.FileEncoding);
        }
    }
}
