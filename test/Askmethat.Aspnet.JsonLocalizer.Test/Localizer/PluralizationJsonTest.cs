using Askmethat.Aspnet.JsonLocalizer.Extensions;
using Askmethat.Aspnet.JsonLocalizer.Localizer;
using Askmethat.Aspnet.JsonLocalizer.Test.Helpers;
using Microsoft.Extensions.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace Askmethat.Aspnet.JsonLocalizer.Test.Localizer
{


    [TestClass]
    public class PluralizationJsonTest
    {
        private JsonStringLocalizer localizer = null;
        public void InitLocalizer(char seperator = '|')
        {
            CultureInfo.CurrentUICulture = new CultureInfo("fr-FR");

            localizer = JsonStringLocalizerHelperFactory.Create(new JsonLocalizationOptions()
            {
                DefaultCulture = new CultureInfo("en-US"),
                SupportedCultureInfos = new System.Collections.Generic.HashSet<CultureInfo>()
                {
                     new CultureInfo("fr-FR"),
                },
                ResourcesPath = "pluralization",
                PluralSeparator = seperator
            });
        }


        [TestMethod]
        public void Should_Be_Singular_Users()
        {
            // Arrange
            InitLocalizer();

            LocalizedString result = localizer.GetString("PluralUser", false);

            Assert.AreEqual("Utilisateur", result);
        }

        [TestMethod]
        public void Should_Be_Plural_Users()
        {
            InitLocalizer();

            LocalizedString result = localizer.GetString("PluralUser", true);

            Assert.AreEqual("Utilisateurs", result);
        }

        [TestMethod]
        public void Should_Be_PluralWithNoSeperator_ShowDefault()
        {
            InitLocalizer();

            LocalizedString result = localizer.GetString("PluralUserFailed", true);

            Assert.AreEqual("Utilisateurs", result);
        }

        [TestMethod]
        public void Should_Be_Singular_Users_Custom()
        {
            // Arrange
            InitLocalizer('#');

            LocalizedString result = localizer.GetString("CustomPluralUser", false);

            Assert.AreEqual("Utilisateur", result);
        }

        [TestMethod]
        public void Should_Be_Plural_Users_Custom()
        {
            // Arrange
            InitLocalizer('#');

            LocalizedString result = localizer.GetString("CustomPluralUser", true);

            Assert.AreEqual("Utilisateurs", result);
        }

        [TestMethod]
        public void Should_Be_Plural_NotFound()
        {
            // Arrange
            InitLocalizer();

            LocalizedString result = localizer.GetString("NotFound", true);

            Assert.AreEqual("NotFound", result);
        }

    }
}
