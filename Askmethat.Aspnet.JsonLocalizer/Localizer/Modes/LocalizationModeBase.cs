using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Askmethat.Aspnet.JsonLocalizer.Format;
using Askmethat.Aspnet.JsonLocalizer.JsonOptions;

namespace Askmethat.Aspnet.JsonLocalizer.Localizer.Modes
{
    internal abstract class LocalizationModeBase
    {
        protected ConcurrentDictionary<string, LocalizatedFormat> localization =
            new ConcurrentDictionary<string, LocalizatedFormat>();

        protected JsonLocalizationOptions _options;
        
        protected void AddOrUpdateLocalizedValue<T>(LocalizatedFormat localizedValue, KeyValuePair<string, T> temp)
        {
            if (!(localizedValue.Value is null))
            {
                if (!localization.ContainsKey(temp.Key))
                {
                    localization.TryAdd(temp.Key, localizedValue);
                }
                else if (localization[temp.Key].IsParent)
                {
                    localization[temp.Key] = localizedValue;
                }
            }
        }
    }
}