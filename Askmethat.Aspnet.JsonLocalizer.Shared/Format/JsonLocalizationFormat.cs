using System.Collections.Generic;

namespace Askmethat.Aspnet.JsonLocalizer.Format
{
    internal class JsonLocalizationFormat
    {
        public Dictionary<string, string> Values { get; set; } = new Dictionary<string, string>();
    }

    internal class LocalizationFormat
    {
        public Dictionary<int, string> Values { get; set; } = new Dictionary<int, string>();
    }

    internal class LocalizatedFormat
    {
        public bool IsParent { get; set; }
        public string Value { get; set; }
    }
}
