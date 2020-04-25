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
        public static ILocalisationModeGenerator GetLocalisationFromMode(
            LocalizationMode localizationMode)
        {
            ILocalisationModeGenerator localisationModeGenerator = null;

            switch (localizationMode)
            {
                case LocalizationMode.I18n:
                    localisationModeGenerator = new LocalisationI18nModeGenerator();
                    break;

                default:
                    localisationModeGenerator = new LocalisationBasicModeGenerator();
                    break;
            };

            return localisationModeGenerator;
        }
    }
}