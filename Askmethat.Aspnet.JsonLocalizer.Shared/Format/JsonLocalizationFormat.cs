﻿using System.Collections.Generic;

namespace Askmethat.Aspnet.JsonLocalizer.Format
{
    internal class JsonLocalizationFormat
    {
        public Dictionary<string, string> Values = new Dictionary<string, string>();
    }

    internal class LocalizationFormat
    {
        public Dictionary<int, string> Values = new Dictionary<int, string>();
    }

    internal class LocalizatedFormat
    {
        public bool IsParent { get; set; }
        public string Value { get; set; }
    }
}
