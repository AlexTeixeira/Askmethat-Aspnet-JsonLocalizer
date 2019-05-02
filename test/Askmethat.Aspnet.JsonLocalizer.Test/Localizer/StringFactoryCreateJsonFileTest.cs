using Askmethat.Aspnet.JsonLocalizer.Extensions;
using Askmethat.Aspnet.JsonLocalizer.Localizer;
using Askmethat.Aspnet.JsonLocalizer.Test.Helpers;
using Microsoft.Extensions.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace Askmethat.Aspnet.JsonLocalizer.Test.Localizer
{
    [TestClass]
    public class StringFactoryCreateJsonFileTest
    {
        JsonStringLocalizer localizer = null;
        public void InitLocalizer(CultureInfo cultureInfo, string baseName = null)
        {
            CultureInfo.CurrentUICulture = cultureInfo;
            localizer = JsonStringLocalizerHelperFactory.Create(new JsonLocalizationOptions()
            {
                DefaultCulture = new CultureInfo("en-US"),
                SupportedCultureInfos = new System.Collections.Generic.HashSet<CultureInfo>()
                {
                     new CultureInfo("fr-FR")
                },
                ResourcesPath = "factory",
                UseBaseName = true
            }, baseName);
        }

        [TestMethod]
        public void TestReadName1_StringLocation()
        {
            InitLocalizer(new CultureInfo("fr-FR"));
            var result = localizer.GetString("Name1");

            Assert.AreEqual("Mon Nom 1", result);
        }

        [TestMethod]
        public void TestReadName1_BaseName_StringLocation()
        {
            InitLocalizer(new CultureInfo("fr-FR"), "base");
            var result = localizer.GetString("Name3");

            Assert.AreEqual("Mon Nom 3", result);

            result = localizer.GetString("Name4");

            Assert.AreEqual("Mon Nom 4", result);

            result = localizer.GetString("Name1");

            Assert.IsTrue(result.ResourceNotFound);
        }
        
    }
}