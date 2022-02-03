using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Localization;
#if NETCORE
using Microsoft.AspNetCore.Components;
#endif

namespace Askmethat.Aspnet.JsonLocalizer.Localizer
{
    public interface IJsonStringLocalizer: IStringLocalizer
    {
	    void ClearMemCache(IEnumerable<CultureInfo> culturesToClearFromCache = null);
	    void ReloadMemCache(IEnumerable<CultureInfo> culturesToClearFromCache = null);
	    IStringLocalizer WithCulture(CultureInfo culture);
	    LocalizedString GetPlural(string key, double count, params object[] arguments);
#if NETCORE
	    MarkupString GetHtmlBlazorString(string name, bool shouldTryDefaultCulture = true);
#endif
    }

    public interface IJsonStringLocalizer<out T> : IJsonStringLocalizer
    {
        
    }
}