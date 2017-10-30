using System;
using System.Collections.Generic;
using System.Text;

namespace Askmethat.Aspnet.JsonLocalizer.Format
{
    internal class JsonLocalizationFormat
    {
        public string Key { get; set; }
        public Dictionary<string, string> Values = new Dictionary<string, string>();
    }
}
