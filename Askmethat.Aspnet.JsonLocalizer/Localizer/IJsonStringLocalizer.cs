using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Askmethat.Aspnet.JsonLocalizer.Localizer
{
    public interface IJsonStringLocalizer: IStringLocalizer
    {
	    void ClearMemCache(IEnumerable<CultureInfo> culturesToClearFromCache = null);
	    void ReloadMemCache(IEnumerable<CultureInfo> culturesToClearFromCache = null);
	    new IStringLocalizer WithCulture(CultureInfo culture);
	    LocalizedString GetPlural(string key, double count, params object[] arguments);
	    MarkupString GetHtmlBlazorString(string name, bool shouldTryDefaultCulture = true);
    }

    public interface IJsonStringLocalizer<out T> : IJsonStringLocalizer
    {
        
    }
}