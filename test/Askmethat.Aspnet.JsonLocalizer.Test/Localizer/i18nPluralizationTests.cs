using Askmethat.Aspnet.JsonLocalizer.Localizer;
using Askmethat.Aspnet.JsonLocalizer.Test.Helpers;
using Microsoft.Extensions.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using Askmethat.Aspnet.JsonLocalizer.JsonOptions;

namespace Askmethat.Aspnet.JsonLocalizer.Test.Localizer
{
    [TestClass]
    public class i18nPluralizationTests
    {
        private JsonStringLocalizer localizer = null;
        public void InitLocalizer(char seperator = '|', string currentCulture = "en-US")
        {
            CultureInfo.CurrentUICulture = new CultureInfo(currentCulture);

            localizer = JsonStringLocalizerHelperFactory.Create(new JsonLocalizationOptions()
            {
                DefaultCulture = new CultureInfo("en-US"),
                SupportedCultureInfos = new System.Collections.Generic.HashSet<CultureInfo>()
                {
                     new CultureInfo("fr-FR"),
                     new CultureInfo("es-UY"),
                },
                ResourcesPath = "i18nPluralization",
                PluralSeparator = seperator,
                LocalizationMode = LocalizationMode.I18n
            });
        }


        [TestMethod]
        public void Should_Be_Zero_Test()
        {
            // Arrange
            InitLocalizer();

            LocalizedString result = localizer.GetPlural("UseCase.Title", 0);

            Assert.AreEqual("No Numbers", result);
        }

        [TestMethod]
        public void Should_Be_One_Test()
        {
            InitLocalizer();
            LocalizedString result = localizer.GetPlural("UseCase.Title", 1);
            Assert.AreEqual("A Number", result);
        }

        [TestMethod]
        public void Should_Be_Other_Test()
        {
            InitLocalizer();
            LocalizedString result = localizer.GetPlural("UseCase.Title", 2);
            Assert.AreEqual("2 Numbers", result);
        }


        [TestMethod]
        public void Should_Be_Other_Test_With_Zero()
        {
            InitLocalizer();
            LocalizedString result = localizer.GetPlural("Title", 0);
            Assert.AreEqual("0 Users", result);
        }

        [TestMethod]
        public void Should_Be_String_No_Pluralization()
        {
            InitLocalizer();
            LocalizedString result = localizer.GetPlural("TextWithNoPlural", 42);
            Assert.AreEqual("Sample Text", result);
        }

        [TestMethod]
        public void Should_Be_Name_If_Not_Defined()
        {
            InitLocalizer();
            LocalizedString result = localizer.GetPlural("Inexistent.Text", 32);
            Assert.AreEqual("Inexistent.Text", result);
        }

        [TestMethod]
        public void Should_Be_Zero_In_Spanish()
        {
            InitLocalizer(currentCulture: "es-UY");
            LocalizedString result = localizer.GetPlural("UseCase.Title", 0);
            Assert.AreEqual("Sin números", result);
        }

    }
}
