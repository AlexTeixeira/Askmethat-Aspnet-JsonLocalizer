using System.Collections.Concurrent;
using System.IO;
using System.Text;
using Askmethat.Aspnet.JsonLocalizer.JsonOptions;
using Newtonsoft.Json;

namespace Askmethat.Aspnet.JsonLocalizer.Localizer.Modes
{
    public static class LocalisationModeHelpers
    {
        public static ConcurrentDictionary<T, U> ReadAndDeserializeFile<T,U>(string file, Encoding encoding)
        {
            return 
                JsonConvert.DeserializeObject<ConcurrentDictionary<T, U>>(
                    File.ReadAllText(file, encoding));
        }
    }
}