using Askmethat.Aspnet.JsonLocalizer.Localizer;
using Askmethat.Aspnet.JsonLocalizer.Test.Helpers;
using Microsoft.Extensions.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using Askmethat.Aspnet.JsonLocalizer.JsonOptions;

namespace Askmethat.Aspnet.JsonLocalizer.Test.Localizer
{
    [TestClass]
    public class AbsoluteMultiplePathJsonFileTest
    {
        private JsonStringLocalizer localizer = null;
        public void InitLocalizer(CultureInfo cultureInfo)
        {
            CultureInfo.CurrentUICulture = cultureInfo;
            localizer = JsonStringLocalizerHelperFactory.Create(new JsonLocalizationOptions()
            {
                DefaultCulture = new CultureInfo("en-US"),
                SupportedCultureInfos = new System.Collections.Generic.HashSet<CultureInfo>()
                {
                     new CultureInfo("fr-FR")
                },
                ResourcesPath = $"{AppContext.BaseDirectory}/path",
                AdditionalResourcePaths = new[] { $"{AppContext.BaseDirectory}/path2" } ,
                IsAbsolutePath = true
            });
        }

        [TestMethod]
        public void TestReadName1_AbsolutePath_StringLocation()
        {
            InitLocalizer(new CultureInfo("fr-FR"));

            LocalizedString result = localizer.GetString("Name1");            

            Assert.AreEqual("Mon Nom 1", result);

            LocalizedString result2 = localizer.GetString("Name3");

            Assert.AreEqual("Mon Nom 3", result2);
        }

    }
}