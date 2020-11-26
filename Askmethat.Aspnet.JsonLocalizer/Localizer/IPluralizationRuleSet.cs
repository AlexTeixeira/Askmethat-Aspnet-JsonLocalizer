using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Askmethat.Aspnet.JsonLocalizer.Localizer
{
    public interface IPluralizationRuleSet
    {
        string GetMatchingPluralizationRule(double count);
    }
}
