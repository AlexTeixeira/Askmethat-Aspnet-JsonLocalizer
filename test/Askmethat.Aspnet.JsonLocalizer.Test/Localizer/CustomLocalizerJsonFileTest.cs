using Askmethat.Aspnet.JsonLocalizer.Extensions;
using Askmethat.Aspnet.JsonLocalizer.Test.Helpers;
using Microsoft.Extensions.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace Askmethat.Aspnet.JsonLocalizer.Test.Localizer
{
    [TestClass]
    public class CustomLocalizerJsonFileTest
    {

        [TestMethod]
        public void Should_Be_Singular_Users_Custom()
        {
            // Arrange
            CultureInfo.CurrentUICulture = new CultureInfo("fr-FR");
            var localizer = JsonStringLocalizerHelperFactory.Create(new JsonLocalizationOptions()
            {
                DefaultCulture = new CultureInfo("en-US"),
                SupportedCultureInfos = new System.Collections.Generic.HashSet<CultureInfo>()
                {
                     new CultureInfo("fr-FR")
                },
                PluralSeparator = '#'
            });

            var result = localizer.GetString("CustomPluralUser", false);

            Assert.AreEqual("Utilisateur", result);
        }

        [TestMethod]
        public void Should_Be_Plural_Users_Custom()
        {
            // Arrange
            CultureInfo.CurrentUICulture = new CultureInfo("fr-FR");
            var localizer = JsonStringLocalizerHelperFactory.Create(new JsonLocalizationOptions()
            {
                DefaultCulture = new CultureInfo("en-US"),
                SupportedCultureInfos = new System.Collections.Generic.HashSet<CultureInfo>()
                {
                     new CultureInfo("fr-FR")
                },
                PluralSeparator = '#'
            });

            var result = localizer.GetString("CustomPluralUser", true);

            Assert.AreEqual("Utilisateurs", result);
        }
    }
}