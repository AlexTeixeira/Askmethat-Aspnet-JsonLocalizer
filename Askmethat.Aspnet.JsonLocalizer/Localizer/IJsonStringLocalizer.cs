using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Localization;

namespace Askmethat.Aspnet.JsonLocalizer.Localizer
{
    public interface IJsonStringLocalizer: IStringLocalizer
    {
        void ClearMemCache(IEnumerable<CultureInfo> culturesToClearFromCache = null);
    }

    public interface IJsonStringLocalizer<T> : IJsonStringLocalizer
    {
        
    }
}