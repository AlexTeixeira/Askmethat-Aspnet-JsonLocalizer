using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using Askmethat.Aspnet.JsonLocalizer.Format;
using Askmethat.Aspnet.JsonLocalizer.JsonOptions;
using Newtonsoft.Json;

namespace Askmethat.Aspnet.JsonLocalizer.Localizer.Modes
{
    internal static class LocalizationModeFactory
    {
        public static ILocalizationModeGenerator GetLocalisationFromMode(
            LocalizationMode localizationMode)
        {
            ILocalizationModeGenerator localizationModeGenerator = null;

            switch (localizationMode)
            {
                case LocalizationMode.I18n:
                    localizationModeGenerator = new LocalizationI18NModeGenerator();
                    break;

                default:
                    localizationModeGenerator = new LocalizationBasicModeGenerator();
                    break;
            };

            return localizationModeGenerator;
        }
    }
}