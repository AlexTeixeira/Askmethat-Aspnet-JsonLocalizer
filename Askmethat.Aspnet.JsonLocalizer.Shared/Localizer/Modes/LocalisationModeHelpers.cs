using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Text.Json;
using Askmethat.Aspnet.JsonLocalizer.JsonOptions;

namespace Askmethat.Aspnet.JsonLocalizer.Localizer.Modes
{
    public static class LocalisationModeHelpers
    {
        public static ConcurrentDictionary<T, U> ReadAndDeserializeFile<T,U>(string file, Encoding encoding)
        {
            return 
                JsonSerializer.Deserialize<ConcurrentDictionary<T, U>>(
                    File.ReadAllText(file, encoding));
        }
    }
}