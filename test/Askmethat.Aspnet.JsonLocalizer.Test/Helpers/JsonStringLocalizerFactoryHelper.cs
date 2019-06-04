using Askmethat.Aspnet.JsonLocalizer.Extensions;
using Askmethat.Aspnet.JsonLocalizer.Localizer;
using Microsoft.Extensions.Options;

namespace Askmethat.Aspnet.JsonLocalizer.Test.Helpers
{
    internal class JsonStringLocalizerHelperFactory
    {
        public JsonStringLocalizerHelperFactory()
        {
        }

        public static JsonStringLocalizer Create(JsonLocalizationOptions options, string baseName = null)
        {
            return new JsonStringLocalizer(Options.Create(options), new HostingEnvironmentStub(), baseName);
        }
    }
}
