using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using Askmethat.Aspnet.JsonLocalizer.Format;
using Askmethat.Aspnet.JsonLocalizer.JsonOptions;

namespace Askmethat.Aspnet.JsonLocalizer.Localizer.Modes
{
    internal interface ILocalisationModeGenerator
    {
        ConcurrentDictionary<string, LocalizatedFormat> ConstructLocalization(
            IEnumerable<string> myFiles, CultureInfo currentCulture, JsonLocalizationOptions options);

    }
}