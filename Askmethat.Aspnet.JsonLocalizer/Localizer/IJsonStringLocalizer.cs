using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Localization;

namespace Askmethat.Aspnet.JsonLocalizer.Localizer
{
    public interface IJsonStringLocalizer: IStringLocalizer
    {
        void ClearMemCache(IEnumerable<CultureInfo> culturesToClearFromCache = null);
        void ReloadMemCache(IEnumerable<CultureInfo> culturesToClearFromCache = null);
        IStringLocalizer WithCulture(CultureInfo culture);
    }

    public interface IJsonStringLocalizer<T> : IJsonStringLocalizer
    {
        
    }
}