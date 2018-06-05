using Microsoft.Extensions.Localization;
using System;
using System.Globalization;
using System.Text;

namespace Askmethat.Aspnet.JsonLocalizer.Extensions
{
    public class JsonLocalizationOptions : LocalizationOptions
    {
        public TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(30);
        public CultureInfo DefaultCulture { get; set; } = new CultureInfo("en-US");

        public Encoding FileEncoding { get; set; } = Encoding.UTF8;
    }
}
