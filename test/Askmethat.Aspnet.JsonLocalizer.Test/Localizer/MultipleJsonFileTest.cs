using Askmethat.Aspnet.JsonLocalizer.Localizer;
using Askmethat.Aspnet.JsonLocalizer.Test.Helpers;
using Microsoft.Extensions.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using Askmethat.Aspnet.JsonLocalizer.JsonOptions;

namespace Askmethat.Aspnet.JsonLocalizer.Test.Localizer
{
    [TestClass]
    public class MultipleJsonFileTest
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
                     new CultureInfo("fr-FR"),
                     new CultureInfo("pt-PT"),
                     new CultureInfo("it-IT"),
                },
                ResourcesPath = "multiple",
                AdditionalResourcePaths = new[] { "multiple2" },
            });
        }

        [TestMethod]
        public void Should_Read_Name1()
        {
            // Arrange
            InitLocalizer(new CultureInfo("fr-FR"));

            LocalizedString result = localizer.GetString("Name1");

            Assert.AreEqual("Mon Nom 1", result);

            Assert.AreEqual("Mon Nom 3", localizer.GetString("Name3"));
        }


        [TestMethod]
        public void Should_Read_Name1_PT()
        {
            // Arrange
            InitLocalizer(new CultureInfo("pt-PT"));

            LocalizedString result = localizer.GetString("Name1");

            Assert.AreEqual("o meu nome 1", result);

            Assert.AreEqual("o meu nome 3", localizer.GetString("Name3"));
        }

        [TestMethod]
        public void Should_Read_Name2_IT()
        {
            // Arrange
            InitLocalizer(new CultureInfo("it-IT"));

            LocalizedString result = localizer.GetString("Name2");

            Assert.AreEqual("il mio nome 2", result);

            Assert.AreEqual("il mio nome 3", localizer.GetString("Name3"));

        }

    }
}
