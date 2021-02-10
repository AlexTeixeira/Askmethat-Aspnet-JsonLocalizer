using System;
using System.Collections.Generic;
using System.Text;

namespace Askmethat.Aspnet.JsonLocalizer.Localizer
{
    /// <summary>
    /// Default pluralization constants as defined in http://unicode.org/reports/tr35/tr35-numbers.html#Language_Plural_Rules
    /// Other custom constants may be defined in a custom implementation.
    /// </summary>
    public class PluralizationConstants
    {
        public const string Zero = "Zero";
        public const string One = "One";
        public const string Two = "Two";
        public const string Few = "Few";
        public const string Many = "Many";
        public const string Other = "Other";
    }
}
