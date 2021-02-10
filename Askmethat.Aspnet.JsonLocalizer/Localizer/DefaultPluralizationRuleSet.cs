using System;
using System.Collections.Generic;
using System.Text;

namespace Askmethat.Aspnet.JsonLocalizer.Localizer
{
    public class DefaultPluralizationRuleSet : IPluralizationRuleSet
    {
        private Dictionary<string, Predicate<double>> rules;

        public DefaultPluralizationRuleSet()
        {
            rules = new Dictionary<string, Predicate<double>>();
            SetupRules();
        }

        /// <summary>
        /// Override to define any rules for the custom rule set.
        /// </summary>
        protected virtual void SetupRules()
        {
            rules.Add(PluralizationConstants.Zero, ValueMatchesZero);
            rules.Add(PluralizationConstants.One, ValueMatchesOne);
            rules.Add(PluralizationConstants.Other, ValueMatchesOther);
        }

        protected virtual bool ValueMatchesZero(double value)
        {
            return value == 0;
        }

        protected virtual bool ValueMatchesOne(double value)
        {
            return value == 1.0;
        }

        protected virtual bool ValueMatchesOther(double value)
        {
            return value != 1.0;
        }

        public string GetMatchingPluralizationRule(double count)
        {
            foreach (var key in rules.Keys)
            {
                if (rules[key](count))
                {
                    return key;
                }
            }

            //If no match is found, always default to Other.
            return PluralizationConstants.Other;
        }
    }
}
