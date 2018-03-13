using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Askmethat.Aspnet.JsonLocalizer.Extensions
{
    public class JsonLocalizationOptions : LocalizationOptions
    {
        public TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(30);
    }
}
