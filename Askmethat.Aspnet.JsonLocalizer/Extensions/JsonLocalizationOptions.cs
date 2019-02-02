using Microsoft.Extensions.Localization;
using System;
using System.Globalization;
using System.Text;

namespace Askmethat.Aspnet.JsonLocalizer.Extensions
{
    public class JsonLocalizationOptions : LocalizationOptions
    {
        public new string ResourcesPath { get; set; } = "Resources";
        public TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(30);
        public CultureInfo DefaultCulture { get; set; } = new CultureInfo("en-US");

        public bool IsAbsolutePath { get; set; } = false;

        public Encoding FileEncoding { get; set; } = Encoding.UTF8;

        public bool UseBaseName { get; set; } = false;
    }
}
