using Askmethat.Aspnet.JsonLocalizer.Localizer;
using Askmethat.Aspnet.JsonLocalizer.Test.Helpers;
using Microsoft.Extensions.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using Askmethat.Aspnet.JsonLocalizer.JsonOptions;
using LocalizedString = Microsoft.Extensions.Localization.LocalizedString;
using System.IO;

namespace Askmethat.Aspnet.JsonLocalizer.Test.Localizer
{
    [TestClass]
    public class MissingTranslationTest
    {
        private JsonStringLocalizer localizer = null;
        public void InitLocalizer(string cultureString)
        {
            SetCurrentCulture(cultureString);

            localizer = JsonStringLocalizerHelperFactory.Create(new JsonLocalizationOptions()
            {
                DefaultCulture = new CultureInfo("en-AU"),
                MissingTranslationLogBehavior = Extensions.MissingTranslationLogBehavior.CollectToJSON,
                SupportedCultureInfos = new System.Collections.Generic.HashSet<CultureInfo>()
                {
                     new CultureInfo("fr"),
                     new CultureInfo("en"),
                     new CultureInfo("zh-CN"),
                     new CultureInfo("en-AU")
                },
                ResourcesPath = $"{AppContext.BaseDirectory}/i18nFallback",
                LocalizationMode = LocalizationMode.I18n
            });
        }

        [TestMethod]
        public void Should_Track_Colored_NotFound()
        {
            var defaultJsonFile = JsonLocalizationOptions.DEFAULT_MISSING_TRANSLATIONS;
            if (File.Exists(defaultJsonFile))
                File.Delete(defaultJsonFile);
            InitLocalizer("en-AU");
            var result = localizer.GetString("Colored",false);
            Assert.IsTrue(result.ResourceNotFound);
            Assert.IsTrue(File.Exists(defaultJsonFile));
        }

        /// <summary>
        /// LocalizedString doesn't implement the IComparer interface required by CollectionAssert.AreEqual(), so providing one here
        /// </summary>
        private class LocalizedStringComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                LocalizedString lsX = (LocalizedString)x;
                LocalizedString lsY = (LocalizedString)y;
                if (ReferenceEquals(lsX, lsY))
                {
                    return 0;
                }
                if (lsX.Name == lsY.Name && lsX.Value == lsY.Value && lsX.ResourceNotFound == lsY.ResourceNotFound)
                {
                    return 0;
                }
                int result = StringComparer.CurrentCulture.Compare(lsX.Name, lsY.Name);
                if (result != 0)
                {
                    return result;
                }
                result = StringComparer.CurrentCulture.Compare(lsX.Value, lsY.Value);
                return result != 0 ? result : lsX.ResourceNotFound.CompareTo(lsY.ResourceNotFound);
            }
        }

        private void SetCurrentCulture(string cultureName)
            => SetCurrentCulture(new CultureInfo(cultureName));

        private void SetCurrentCulture(CultureInfo cultureInfo)
            => CultureInfo.CurrentUICulture = cultureInfo;
    }
}
